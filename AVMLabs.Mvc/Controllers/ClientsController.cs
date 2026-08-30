using AVMLabs.Mvc.Models;
using AVMLabs.Mvc.Services;
using Microsoft.AspNetCore.Mvc;

namespace AVMLabs.Mvc.Controllers
{
    public class ClientsController : Controller
    {
        private readonly ApiClient _api;

        public ClientsController(ApiClient api)
        {
            _api = api;
        }

        // GET /Clients?search=xyz  -> View 1: Client List Page
        [HttpGet]
        public async Task<IActionResult> Index(string? search)
        {
            var clients = await _api.GetAsync<List<ClientListItemVm>>("api/clients") ?? new();

            if (!string.IsNullOrWhiteSpace(search))
            {
                clients = clients
                    .Where(c => c.ClientName.Contains(search, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            var vm = new ClientListPageVm
            {
                Clients = clients,
                SearchTerm = search
            };

            return View(vm);
        }

        // GET /Clients/Details/5 -> View 2: Client Details Page
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var client = await _api.GetAsync<ClientDetailsApiVm>($"api/clients/{id}");
            if (client == null) return NotFound();

            // Work orders aren't returned by /api/clients/{id}; pull the full list and filter.
            // (In a larger app this would be a dedicated /api/clients/{id}/workorders endpoint.)
            var allOrders = await _api.GetAsync<List<WorkOrderApiVm>>("api/workorders") ?? new();
            var clientOrders = allOrders
                .Where(o => o.ClientName == client.ClientName)
                .Select(o => new WorkOrderRowVm { WODate = o.WODate, Status = o.Status, TotalAmount = o.TotalAmount })
                .ToList();

            var vm = new ClientDetailsVm
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                City = client.City,
                Country = client.Country,
                Email = client.Email,
                Phone = client.Phone,
                WorkOrders = clientOrders
            };

            return View(vm);
        }

        // GET /Clients/Create -> form linked from View 1's "Add New Client" button
        [HttpGet]
        public IActionResult Create() => View(new ClientCreateVm());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClientCreateVm vm)
        {
            if (!ModelState.IsValid) return View(vm);

            var (success, error, _) = await _api.PostAsync<object>("api/clients", vm);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, "Could not create client. " + error);
                return View(vm);
            }

            return RedirectToAction(nameof(Index));
        }

        // Internal helper shapes matching the API's DTOs
        public class ClientDetailsApiVm
        {
            public int ClientId { get; set; }
            public string ClientName { get; set; } = string.Empty;
            public string? City { get; set; }
            public string? Country { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
        }

        public class WorkOrderApiVm
        {
            public string ClientName { get; set; } = string.Empty;
            public DateTime WODate { get; set; }
            public string Status { get; set; } = string.Empty;
            public decimal TotalAmount { get; set; }
        }
    }
}
