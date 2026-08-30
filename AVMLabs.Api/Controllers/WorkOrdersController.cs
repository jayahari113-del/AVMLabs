using AVMLabs.Api.Data;
using AVMLabs.Api.DTOs;
using AVMLabs.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVMLabs.Api.Controllers
{
    [ApiController]
    [Route("api/workorders")]
    public class WorkOrdersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public WorkOrdersController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/workorders - list all work orders with ClientName and TotalAmount
        [HttpGet]
        public async Task<ActionResult<IEnumerable<WorkOrderListDto>>> GetWorkOrders()
        {
            var workOrders = await _context.WorkOrders
                .Include(w => w.Client)
                .OrderByDescending(w => w.WODate)
                .Select(w => new WorkOrderListDto
                {
                    WOId = w.WOId,
                    ClientName = w.Client!.ClientName,
                    WODate = w.WODate,
                    Status = w.Status,
                    TotalAmount = w.TotalAmount
                })
                .ToListAsync();

            return Ok(workOrders);
        }

        // POST /api/workorders - create a new work order with one or more items
        [HttpPost]
        public async Task<ActionResult<WorkOrderResponseDto>> CreateWorkOrder(WorkOrderCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = await _context.Clients.FindAsync(dto.ClientId);
            if (client == null)
                return BadRequest(new { message = $"ClientId {dto.ClientId} does not exist." });

            // Validate every TestId up front and build items with the current DB rate
            var testIds = dto.Items.Select(i => i.TestId).Distinct().ToList();
            var tests = await _context.Tests
                .Where(t => testIds.Contains(t.TestId))
                .ToDictionaryAsync(t => t.TestId);

            var missingTestIds = testIds.Where(id => !tests.ContainsKey(id)).ToList();
            if (missingTestIds.Any())
                return BadRequest(new { message = $"Invalid TestId(s): {string.Join(", ", missingTestIds)}" });

            var workOrder = new WorkOrder
            {
                ClientId = dto.ClientId,
                WODate = DateTime.UtcNow,
                Status = "Pending"
            };

            foreach (var itemDto in dto.Items)
            {
                var test = tests[itemDto.TestId];
                workOrder.Items.Add(new WorkOrderItem
                {
                    TestId = test.TestId,
                    Quantity = itemDto.Quantity,
                    Rate = test.Rate
                });
            }

            workOrder.TotalAmount = workOrder.Items.Sum(i => i.Amount);

            _context.WorkOrders.Add(workOrder);
            await _context.SaveChangesAsync();

            var response = new WorkOrderResponseDto
            {
                WOId = workOrder.WOId,
                ClientId = workOrder.ClientId,
                ClientName = client.ClientName,
                WODate = workOrder.WODate,
                Status = workOrder.Status,
                TotalAmount = workOrder.TotalAmount,
                Items = workOrder.Items.Select(i => new WorkOrderItemResponseDto
                {
                    WOItemId = i.WOItemId,
                    TestId = i.TestId,
                    TestName = tests[i.TestId].TestName,
                    Quantity = i.Quantity,
                    Rate = i.Rate,
                    Amount = i.Amount
                }).ToList()
            };

            return CreatedAtAction(nameof(GetWorkOrders), new { id = workOrder.WOId }, response);
        }
    }
}
