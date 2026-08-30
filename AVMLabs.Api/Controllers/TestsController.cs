using AVMLabs.Api.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVMLabs.Api.Controllers
{
    // Supporting endpoint (not in the marked spec's core 6 routes) used to populate
    // the Test dropdown on the MVC Work Order Entry form.
    [ApiController]
    [Route("api/tests")]
    public class TestsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestsController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetTests()
        {
            var tests = await _context.Tests
                .Where(t => t.IsActive)
                .OrderBy(t => t.TestName)
                .Select(t => new { t.TestId, t.TestName, t.Rate })
                .ToListAsync();

            return Ok(tests);
        }
    }
}
