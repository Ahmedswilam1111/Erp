using Microsoft.AspNetCore.Mvc;

namespace ERPtask.Controllers
{
    using ERPtask.servcies;
    using Microsoft.AspNetCore.Mvc;
    using System.ComponentModel.DataAnnotations;

    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : ControllerBase
    {
        private readonly NotificationService _service;

        public NotificationsController(NotificationService service)
        {
            _service = service;
        }

        [HttpPost("reminder")]
        public async Task<IActionResult> SendReminder([FromBody] ReminderRequest request)
        {
            try
            {
                var notification = await _service.SendDueReminder(request.InvoiceId, request.Type);
                return Ok(notification);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("invoice/{invoiceId}")]
        public async Task<IActionResult> GetInvoiceNotifications(Guid invoiceId)
        {
            var notifications = await _service.GetInvoiceNotifications(invoiceId);
            return Ok(notifications);
        }

        [HttpPost("retry/{notificationId}")]
        public async Task<IActionResult> RetryNotification(Guid notificationId)
        {
            var result = await _service.RetryFailedNotification(notificationId);
            return result ? Ok() : BadRequest("Retry failed or not applicable");
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetNotification(Guid id)
        {
            var notification = await _service.GetNotificationById(id);
            return notification != null ? Ok(notification) : NotFound();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var result = await _service.DeleteNotification(id);
            return result ? NoContent() : NotFound();
        }
    }

    public class ReminderRequest
    {
        public Guid InvoiceId { get; set; }
        public string Type { get; set; }
    }

    public class NotificationCreateDto
    {
        [Required]
        public string InvoiceId { get; set; }  

        [Required]
        [RegularExpression("Email|SMS|InApp")]
        public string Type { get; set; }
    }

    public class NotificationResponseDto
    {
        public Guid NotificationId { get; set; }
        public Guid InvoiceId { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public DateTime SentDate { get; set; }
    }
}
