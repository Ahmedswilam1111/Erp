using Microsoft.AspNetCore.Mvc;
using ERPtask.servcies;
using ERPtask.models;
using System.ComponentModel.DataAnnotations;
namespace ERPtask.Controllers
{
    

    [ApiController]
    [Route("api/[controller]")]
    public class CostEntriesController : ControllerBase
    {
        private readonly CostEntryService _service;

        public CostEntriesController(CostEntryService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> CreateCostEntry([FromBody] CostEntry entry)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEntry = await _service.CreateCostEntry(entry);
            return CreatedAtAction(nameof(GetCostEntry), new { id = createdEntry.CostEntryId }, createdEntry);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetCostEntry(Guid id)
        {
            var entry = await _service.GetCostEntryById(id);
            return entry != null ? Ok(entry) : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllCostEntries()
        {
            var entries = await _service.GetAllCostEntries();
            return Ok(entries);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCostEntry(Guid id, [FromBody] CostEntry entry)
        {
            if (id != entry.CostEntryId)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _service.UpdateCostEntry(entry);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCostEntry(Guid id)
        {
            var result = await _service.DeleteCostEntry(id);
            return result ? NoContent() : NotFound();
        }
    }
    public class CostEntryCreateDto
    {
        [Required]
        [StringLength(500)]
        public string CostCategory { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal Amount { get; set; }

        public DateTime Date { get; set; }

        [StringLength(500)]
        public string Description { get; set; }
    }
    public class CostEntryResponseDto
    {
        public Guid CostEntryId { get; set; }
        public string CostCategory { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
    }
}
