namespace Inspire.Erp.NotificationBus
{
    public interface INotificationHub
    {
        Task BroadcastToUser(string data, string userId);
        Task SendMessage(string user, string message);
        Task Send(string user, ApiMethod client);
        Task SendApprovalNotification(string userConnectionId,object obj);
    }

}
