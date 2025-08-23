using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Application.CusNotification;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Inspire.Erp.Web.Controllers.Notifications
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationsController : ControllerBase
    {
        private readonly INotificaionService _notificationService;

        public NotificationsController(INotificaionService notificationService)
        {
            _notificationService = notificationService;
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


    }
}
