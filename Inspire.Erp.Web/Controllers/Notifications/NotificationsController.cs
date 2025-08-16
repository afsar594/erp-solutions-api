using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Application.CusNotification;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

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
