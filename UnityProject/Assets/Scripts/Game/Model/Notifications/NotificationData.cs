namespace Game.Models.Notifications
{
    public class NotificationData
    {
        public readonly int ID;
        public readonly LocalizationKeyID Title;
        public readonly LocalizationKeyID Text;
        public readonly LocalNotificationID LocalNotificationID;
        public readonly int WaitInterval;

        public NotificationData(LocalizationKeyID title, LocalizationKeyID text, LocalNotificationID localNotificationID, int id, int waitInterval)
        {
            Title = title;
            Text = text;
            LocalNotificationID = localNotificationID;
            ID = id;
            WaitInterval = waitInterval;
        }

        public NotificationData(NotificationData notificationData, int id)
        {
            Title = notificationData.Title;
            Text = notificationData.Text;
            LocalNotificationID = notificationData.LocalNotificationID;
            WaitInterval = notificationData.WaitInterval;
            
            ID = id;
        }
    }
}