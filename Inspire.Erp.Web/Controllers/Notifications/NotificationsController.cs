using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Application.CusNotification;
using Inspire.Erp.Application.Email.Interfaces;
using Inspire.Erp.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificaionService _notificationService;
        private readonly IOrderEmailNotificationService _emailNotificationService;

        public NotificationsController(INotificaionService notificationService, IOrderEmailNotificationService emailNotificationService)
        {
            _notificationService = notificationService;
            _emailNotificationService = emailNotificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications()
        {
            return Ok(await _notificationService.GetNotificationsAsync());
        }

        [HttpPost("{Id}")]
        public async Task<IActionResult> SaveNotification([FromBody] NotificationDto notification, int Id)
        {
            if (Id != notification.NotificationId)
                return BadRequest("Id mismatched");

            return Ok(await _notificationService.CreateNotificationAsync(notification));
        }

        [HttpPost]
        [Route("resend-email")]
        public async Task<IActionResult> ResendEmail([FromQuery] EnumOrderForms type, [FromQuery] EnumApproverLevels level, [FromQuery] long orderId)
        {
            return Ok(await _emailNotificationService.ResendEmailAsync(type, level, orderId));
        }


    }
}
