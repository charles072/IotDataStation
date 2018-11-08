namespace IotDataServer.Notification
{
    public interface INotificationObserver
    {
        void OnReceiveNotificationMessage(NotificationMessage notificationMessage);
    }
}