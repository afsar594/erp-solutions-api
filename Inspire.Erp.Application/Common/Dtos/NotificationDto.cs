using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.Common.Dtos
{
    public class NotificationDto
    {
        public int NotificationId { get; set; }
        public string Message { get; set; }
        public string RouteUrl { get; set; }
        public bool IsRead { get; set; }
        public long? UserId { get; set; }
    }
}
