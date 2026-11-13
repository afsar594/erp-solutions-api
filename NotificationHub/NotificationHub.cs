using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inspire.Erp.Notifications
{
    public class NotificationHub : Hub, INotificationHub
    {
        private readonly IHubContext<NotificationHub> _hub;
        public NotificationHub(IHubContext<NotificationHub> hub)
        {
            _hub = hub;
        }

        public async Task BroadcastToUser(string data, string userId)
            => await _hub.Clients.All.SendAsync("ReceiveMessage", userId, data);

        public string GetConnectionId() => Context.ConnectionId;
        public string GetUserId() => Context.User.Identity.Name;

        public async Task SendMessage(string user, string message)
        {
            await _hub.Clients.Client(user).SendAsync("ReceiveMessage", user, message);
        }

        public async Task Send(string user, ApiMethod client)
        {
            await _hub.Clients.Client(user).SendAsync("ReceiveClient", user, client);
        }

        /// <summary>
        /// Sends a PO/PR approval pending notification to the concerned user
        /// </summary>
        /// <param name="userConnectionId">The SignalR connection ID of the user</param>
        /// <param name="poOrPrNumber">The PO or PR number created</param>
        /// <param name="creatorName">The name of the person who created it</param>
        public async Task SendApprovalNotification(string userConnectionId, string poPrType, string poOrPrNumber, string creatorName)
        {
            string message = $"{poPrType} against '{poOrPrNumber}' has been created by {creatorName} and your approval is pending. Please approve it as soon as possible.";
            await _hub.Clients.Client(userConnectionId).SendAsync("ReceiveNotification", message);
        }

        public Task SendApprovalNotification(string userConnectionId, string poOrPrNumber, string creatorName)
        {
            throw new System.NotImplementedException();
        }
    }

    public interface INotificationHub
    {
        Task BroadcastToUser(string data, string userId);
        Task SendMessage(string user, string message);
        Task Send(string user, ApiMethod client);
        Task SendApprovalNotification(string userConnectionId, string poOrPrNumber, string creatorName);
    }

}
