using Microsoft.AspNetCore.Mvc;
using ERPtask.models;
using ERPtask.servcies;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
namespace ERPtask.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InvoicesController : ControllerBase
    {
        private readonly InvoiceService _invoiceService;

        public InvoicesController(InvoiceService invoiceService)
        {
            _invoiceService = invoiceService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateInvoice([FromBody] InvoiceCreateDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Guid.TryParse(request.ClientId, out Guid clientId))
            {
                return BadRequest("Invalid Client ID format");
            }

            try
            {
                var invoice = await _invoiceService.GenerateInvoice(
                    clientId,
                    request.Items,
                    request.Region,
                    request.Discounts);

                var responseDto = new InvoiceResponseDto
                {
                    InvoiceId = invoice.InvoiceId,
                    ClientId = invoice.ClientId,
                    Subtotal = invoice.Subtotal,
                    Tax = invoice.Tax,
                    Discounts = invoice.Discounts,
                    Total = invoice.Total,
                    DueDate = invoice.DueDate,
                    Status = invoice.Status,
                    Items = invoice.Items.Select(i => new InvoiceItemResponseDto
                    {
                        ItemId = i.ItemId,
                        Name = i.Name,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice
                    }).ToList()
                };

                return CreatedAtAction(
                    nameof(GetInvoice),
                    new { id = responseDto.InvoiceId },
                    responseDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetInvoice(Guid id)
        {
            var invoice = await _invoiceService.GetInvoiceById(id);
            return invoice != null ? Ok(invoice) : NotFound();
        }

        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] string status)//('Draft', 'Sent', 'Paid', 'Overdue')
        {
            var result = await _invoiceService.UpdateInvoiceStatus(id, status);
            return result ? NoContent() : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(Guid id)
        {
            var result = await _invoiceService.DeleteInvoice(id);
            return result ? NoContent() : NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllInvoices()
        {
            try
            {
                var invoices = await _invoiceService.GetAllInvoices();
                var response = invoices.Select(i => new InvoiceResponseDto
                {
                    InvoiceId = i.InvoiceId,
                    ClientId = i.ClientId,
                    Subtotal = i.Subtotal,
                    Tax = i.Tax,
                    Discounts = i.Discounts,
                    Total = i.Total,
                    DueDate = i.DueDate,
                    Status = i.Status,
                    Items = i.Items.Select(item => new InvoiceItemResponseDto
                    {
                        ItemId = item.ItemId,
                        Name = item.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    }).ToList()
                }).ToList();

                return Ok(response);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving invoices");
            }
        }
    }


    public class InvoiceRequest
    {
        public Guid ClientId { get; set; }
        public List<InvoiceItemRequest> Items { get; set; }
        public string Region { get; set; }
        public decimal Discounts { get; set; }
    }

    public class InvoiceItemRequest
    {
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

    public class InvoiceCreateDto
    {
        [Required]
        public string ClientId { get; set; }  // Sent as string, parsed to Guid

        [Required]
        [MinLength(1)]
        public List<InvoiceItemCreateDto> Items { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Discounts { get; set; }

        [Required]
        public string Region { get; set; }
    }

    public class InvoiceResponseDto
    {
        public Guid InvoiceId { get; set; }
        public Guid ClientId { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Tax { get; set; }
        public decimal Discounts { get; set; }
        public decimal Total { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public List<InvoiceItemResponseDto> Items { get; set; }
    }
    // Invoice Item DTOs
    public class InvoiceItemCreateDto
    {
        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0.01, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }

    public class InvoiceItemResponseDto
    {
        public Guid ItemId { get; set; }
        public string Name { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }

}
