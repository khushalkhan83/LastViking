using System.Collections.Generic;


namespace Game.Models.Notifications
{
    public class NotificationsResponce
    {
        public readonly NotificationData notification; // original notification (krab arrived on island. Go find him)
        public readonly IEnumerable<NotificationData> reminders; // sended after activity is avaliable. (krab is still waiting for you)

        public NotificationsResponce(NotificationData notification, IEnumerable<NotificationData> reminders)
        {
            this.notification = notification;
            this.reminders = reminders;
        }
    }
}