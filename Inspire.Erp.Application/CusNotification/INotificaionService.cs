using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Inspire.Erp.Application.CusNotification
{
    public interface INotificaionService
    {
        public Task<ApiResponse<List<NotificationDto>>> GetNotificationsAsync(CancellationToken cancellationToken = default);
        public Task CreateNotificationAsync(NotificationDto dto, CancellationToken cancellationToken = default);
    }
}
