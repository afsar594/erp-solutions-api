using AutoMapper;
using Dapper;
using Inspire.Erp.Application.Common.Dtos;
using Inspire.Erp.Application.Email.Implementations;
using Inspire.Erp.Application.User;
using Inspire.Erp.Domain.Entities;
using Inspire.Erp.Domain.Models;
using Inspire.Erp.Infrastructure;
using Inspire.Erp.Infrastructure.Database.Repositoy;
using Inspire.Erp.NotificationBus;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<NotificaionService> _logger;
        private readonly INotificationHub _notificationHub;
        
        public NotificaionService(IOptions<ApplicationSettings> applicationSettings, IRepository<Notification> repository, ICurrentUserService currentUserService, INotificationHub notificationHub, ILogger<NotificaionService> logger)
        {
            _appSettings = applicationSettings.Value;
            _repository = repository;
            _currentUserService = currentUserService;
            _notificationHub = notificationHub;
            _logger = logger;
        }
        public async Task<ApiResponse<bool>> CreateNotificationAsync(NotificationDto dto)
        {
            try
            {
                if (dto == null)
                    return ApiResponse<bool>.Fail("Invalid notification data.");

                if (dto.NotificationId == 0)
                {
                    // Insert new notification
                    var notiToAdd = new Notification
                    {
                        IsRead = dto.IsRead,
                        Message = dto.Message,
                        RouteUrl = dto.RouteUrl,
                        UserId = dto.UserId ?? _currentUserService.UserId,
                    };

                    await _repository.InsertAsync(notiToAdd);
                }
                else
                {
                    // Update existing notification
                    var notiToUpdate = await _repository
                        .GetAsQueryable()
                        .FirstOrDefaultAsync(x => x.NotificationId == dto.NotificationId);

                    if (notiToUpdate == null)
                    {
                        // Fallback: create new if not found
                        var notiToAdd = new Notification
                        {
                            IsRead = dto.IsRead,
                            Message = dto.Message,
                            RouteUrl = dto.RouteUrl,
                            UserId = dto.UserId ?? _currentUserService.UserId,
                        };

                        await _repository.InsertAsync(notiToAdd);
                    }
                    else
                    {
                        notiToUpdate.IsRead = true;
                        await _repository.UpdateAsync(notiToUpdate);
                    }
                }
                await notifyUser(dto);
                return ApiResponse<bool>.Success(true);
            }
            catch (Exception ex)
            {
                // Log the error
                return ApiResponse<bool>.Fail($"Error creating/updating notification: {ex.Message}");
            }
        }


        public async Task<ApiResponse<List<NotificationDto>>> GetNotificationsAsync()
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


        private async Task notifyUser(NotificationDto notification)
        {
            try
            {
                await _notificationHub.SendApprovalNotification(notification, notification.UserId);
                _logger.LogInformation("Notification Sent {notificationToAdd}", notification);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
