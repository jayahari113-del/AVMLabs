using AVMLabs.Api.Data;
using AVMLabs.Api.DTOs;
using AVMLabs.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AVMLabs.Api.Controllers
{
    [ApiController]
    [Route("api/clients")]
    public class ClientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClientsController(AppDbContext context)
        {
            _context = context;
        }

        // GET /api/clients - list all active clients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClientListDto>>> GetClients()
        {
            var clients = await _context.Clients
                .Where(c => c.IsActive)
                .OrderBy(c => c.ClientName)
                .Select(c => new ClientListDto
                {
                    ClientId = c.ClientId,
                    ClientName = c.ClientName,
                    City = c.City,
                    Country = c.Country
                })
                .ToListAsync();

            return Ok(clients);
        }

        // GET /api/clients/{id} - get a single client (404 if not found)
        [HttpGet("{id:int}")]
        public async Task<ActionResult<ClientDetailDto>> GetClient(int id)
        {
            var client = await _context.Clients.FindAsync(id);

            if (client == null)
                return NotFound(new { message = $"Client with Id {id} was not found." });

            return Ok(new ClientDetailDto
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ContactPerson = client.ContactPerson,
                Phone = client.Phone,
                Email = client.Email,
                City = client.City,
                Country = client.Country,
                IsActive = client.IsActive
            });
        }

        // POST /api/clients - create a new client (basic validation: Email format required)
        [HttpPost]
        public async Task<ActionResult<ClientDetailDto>> CreateClient(ClientCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = new Client
            {
                ClientName = dto.ClientName,
                ContactPerson = dto.ContactPerson,
                Phone = dto.Phone,
                Email = dto.Email,
                City = dto.City,
                Country = dto.Country,
                IsActive = true
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            var result = new ClientDetailDto
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                ContactPerson = client.ContactPerson,
                Phone = client.Phone,
                Email = client.Email,
                City = client.City,
                Country = client.Country,
                IsActive = client.IsActive
            };

            return CreatedAtAction(nameof(GetClient), new { id = client.ClientId }, result);
        }

        // PUT /api/clients/{id} - update an existing client's details
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateClient(int id, ClientUpdateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return NotFound(new { message = $"Client with Id {id} was not found." });

            client.ClientName = dto.ClientName;
            client.ContactPerson = dto.ContactPerson;
            client.Phone = dto.Phone;
            client.Email = dto.Email;
            client.City = dto.City;
            client.Country = dto.Country;
            client.IsActive = dto.IsActive;

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
