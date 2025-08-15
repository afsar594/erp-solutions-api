using AutoMapper;
using Dapper;
using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Application.User;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Dapper.SqlMapper;

namespace Inspire.Erp.Application.CusNotification
{
    public class NotificaionService : INotificaionService
    {
        private readonly IRepository<Notification> _repository;
        private readonly ApplicationSettings _appSettings;
        private readonly ICurrentUserService _currentUserService;
        public NotificaionService(IOptions<ApplicationSettings> applicationSettings, IRepository<Notification> repository, ICurrentUserService currentUserService)
        {
            _appSettings = applicationSettings.Value;
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public async Task CreateNotificationAsync(NotificationDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                Notification notiToAddUpdate;
                if (dto.NotificationId == 0)
                {
                    notiToAddUpdate = new Notification()
                    {
                        IsRead = dto.IsRead,
                        Message = dto.Message,
                        RouteUrl = dto.RouteUrl,
                        UserId = dto.UserId
                    };
                    await _repository.InsertAsync(notiToAddUpdate);
                }
                else
                {
                    notiToAddUpdate = new Notification()
                    {
                        NotificationId = dto.NotificationId,
                        IsRead = dto.IsRead,
                        Message = dto.Message,
                        RouteUrl = dto.RouteUrl,
                        UserId = dto.UserId
                    };
                    await _repository.UpdateAsync(notiToAddUpdate);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ApiResponse<List<NotificationDto>>> GetNotificationsAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                using (var _dbConnection = new SqlConnection(_appSettings.ConnectionString))
                {
                    var sql = @"SELECT TOP 20 NotificationId, Message, RouteUrl, IsRead
                                        FROM Notifications
                                        WHERE IsRead = 0 and UserId = @UserId
                                        ORDER BY CreatedAt DESC";

                    var notifications = (await _dbConnection.QueryAsync<NotificationDto>(sql, new { UserId = _currentUserService.UserId })).ToList();
                    if (!notifications.Any())
                        return ApiResponse<List<NotificationDto>>.Fail("No notifications were found.");
                    return ApiResponse<List<NotificationDto>>.Success(notifications, "Notifications were found.");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
