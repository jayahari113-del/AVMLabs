using AVMLabs.Api.Data;
using AVMLabs.Api.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVMLabs.Api.Controllers
{
    [ApiController]
    [Route("api/reports")]
    public class ReportsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/reports/client-summary
        // Total work order amount per client, grouped and sorted descending
        [HttpGet("client-summary")]
        public async Task<ActionResult<IEnumerable<ClientSummaryDto>>> GetClientSummary()
        {
            var summary = await _context.WorkOrders
                .Include(w => w.Client)
                .GroupBy(w => w.Client!.ClientName)
                .Select(g => new ClientSummaryDto
                {
                    ClientName = g.Key,
                    TotalWorkOrderAmount = g.Sum(w => w.TotalAmount)
                })
                .OrderByDescending(s => s.TotalWorkOrderAmount)
                .ToListAsync();

            return Ok(summary);
        }
    }
}
