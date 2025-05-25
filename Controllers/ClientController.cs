using ERPtask.models;
using ERPtask.servcies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ERPtask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly ClientService _service;

        public ClientsController(ClientService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllClients()
        {
            var clients = await _service.GetAllClients();
            return Ok(clients);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetClient(Guid id)
        {
            var client = await _service.GetClient(id);
            return client != null ? Ok(client) : NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> CreateClient([FromBody] ClientCreateDto clientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var client = new Client
            {
                Email = clientDto.Email,
                Phone = clientDto.Phone,
                Address = clientDto.Address
            };

            var createdClient = await _service.CreateClient(client);
            return CreatedAtAction(nameof(GetClient), new { id = createdClient.ClientId }, createdClient);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateClient(Guid id, [FromBody] Client client)
        {
            if (id != client.ClientId)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateClient(client);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteClient(Guid id)
        {
            var result = await _service.DeleteClient(id);
            return result ? NoContent() : NotFound();
        }
    }
    public class ClientCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }
    }

    public class ClientResponseDto
    {
        public Guid ClientId { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
    }

}
