namespace IotDataServer.Common.Notification
{
    public interface INotificationObserver
    {
        void OnReceiveNotificationMessage(NotificationMessage notificationMessage);
    }
}