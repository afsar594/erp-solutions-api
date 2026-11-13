using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Inspire.Erp.NotificationBus
{
    [Authorize]
    public class NotificationHub : Hub, INotificationHub
    {
        // Store mapping between UserId and ConnectionId
        private static ConcurrentDictionary<string, string> _userConnections = new ConcurrentDictionary<string, string>();
        //private readonly ICurrentUserService _currentUserService;

        private readonly IHubContext<NotificationHub> _hub;
        public NotificationHub(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public override Task OnConnectedAsync()
        {
            var userId = Context.UserIdentifier ?? Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections[userId] = Context.ConnectionId;
                Console.WriteLine($"User {userId} connected with ConnectionId {Context.ConnectionId}");
            }

            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            var userId = Context.UserIdentifier ?? Context.User?.Identity?.Name;

            if (!string.IsNullOrEmpty(userId))
            {
                _userConnections.TryRemove(userId, out _);
                Console.WriteLine($"User {userId} disconnected");
            }

            return base.OnDisconnectedAsync(exception);
        }

        public string GetConnectionId() => Context.ConnectionId;
        public string GetUserId() => Context.User?.Identity?.Name;

        public async Task BroadcastToUser(string data, string userId)
            => await _hub.Clients.All.SendAsync("ReceiveMessage", userId, data);

        public async Task SendMessage(string user, string message)
        {
            await _hub.Clients.Client(user).SendAsync("ReceiveMessage", user, message);
        }

        public async Task Send(string user, ApiMethod client)
        {
            await _hub.Clients.Client(user).SendAsync("ReceiveClient", user, client);
        }

        public async Task SendApprovalNotification(object obj, long? userConnectionId = null)
        {
            try
            {
                string json = JsonSerializer.Serialize(obj);
                if (userConnectionId.HasValue)
                    await _hub.Clients.Client(GetConnectionIdByUserId(userConnectionId.Value.ToString())).SendAsync("ReceiveNotification", json);
                else
                    await _hub.Clients.All.SendAsync("ReceiveNotification", JsonSerializer.Serialize(obj));
            }
            catch (Exception)
            {
            }

        }


        public static string GetConnectionIdByUserId(string userId)
        {
            _userConnections.TryGetValue(userId, out var connectionId);
            return connectionId;
        }
    }
}
