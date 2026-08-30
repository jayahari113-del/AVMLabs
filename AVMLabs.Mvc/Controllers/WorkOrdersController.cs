using AVMLabs.Mvc.Models;
using AVMLabs.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace AVMLabs.Mvc.Controllers
{
    public class WorkOrdersController : Controller
    {
        private readonly ApiClient _api;

        public WorkOrdersController(ApiClient api)
        {
            _api = api;
        }

        // GET /WorkOrders/Create -> View 3: Work Order Entry Form
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var vm = await BuildVmAsync();
            return View(vm);
        }

        // POST /WorkOrders/Create -> submits to the Web API's POST /api/workorders
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int clientId, List<int> testIds, List<int> quantities)
        {
            var vm = await BuildVmAsync();
            vm.ClientId = clientId;

            if (clientId == 0 || testIds == null || !testIds.Any())
            {
                vm.Message = "Please select a client and add at least one test.";
                vm.Success = false;
                return View(vm);
            }

            var items = testIds
                .Select((testId, idx) => new { TestId = testId, Quantity = quantities.ElementAtOrDefault(idx) <= 0 ? 1 : quantities.ElementAtOrDefault(idx) })
                .ToList();

            var payload = new { ClientId = clientId, Items = items };

            var (success, error, _) = await _api.PostAsync<object>("api/workorders", payload);

            vm.Success = success;
            vm.Message = success
                ? "Work order created successfully."
                : $"Failed to create work order: {error}";

            return View(vm);
        }

        private async Task<WorkOrderEntryVm> BuildVmAsync()
        {
            var clients = await _api.GetAsync<List<ClientListItemVm>>("api/clients") ?? new();
            var tests = await _api.GetAsync<List<TestOptionVm>>("api/tests") ?? new();

            return new WorkOrderEntryVm
            {
                Clients = clients,
                Tests = tests
            };
        }
    }
}
