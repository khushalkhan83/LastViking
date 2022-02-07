using System.Collections.Generic;
using UnityEngine;
using System;


namespace Game.Models.Notifications
{
    [Serializable]
    public class SpecialNotificationsModel
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private int _startIndex;
        [SerializeField] private List<NotificationGroup> notifications;
        
        #pragma warning restore 0649
        #endregion

        private Dictionary<LocalNotificationID,NotificationsResponce> responceByGroup;

        public NotificationsResponce GetNotificationsRequest(LocalNotificationID notificationID)
        {
            if(responceByGroup == null) CacheNotifications();

            var answer = responceByGroup[notificationID];
            return answer;
        }

        public bool TryGetLocalNotifiicationID(int id, out LocalNotificationID localNotificationID)
        {
            localNotificationID = LocalNotificationID.None;

            if(responceByGroup == null) CacheNotifications();

            foreach (var responce in responceByGroup)
            {
                if(responce.Value.notification.ID == id)
                {
                    localNotificationID = responce.Value.notification.LocalNotificationID;
                    return true;
                }

                foreach (var reminder in responce.Value.reminders)
                {
                    if(reminder.ID == id)
                    {
                        localNotificationID = reminder.LocalNotificationID;
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// indexes example
        /// kill krab reminder ID = 1000
        /// kill krab reminder ID = 1001
        /// kill krab reminder ID = 1002
        /// take chest reminder ID = 1003
        /// take chest reminder ID = 1004
        /// take chest reminder ID = 1005
        /// </summary>
        private void CacheNotifications()
        {
            responceByGroup = new Dictionary<LocalNotificationID, NotificationsResponce>();
            
            var index = _startIndex;

            foreach (var group in notifications)
            {
                var reminders = new List<NotificationData>();

                foreach (var notificationData in group.GetReminders(index))
                {
                    reminders.Add(notificationData);
                    index = notificationData.ID + 1;
                }
                
                var regularNotification = group.GetRegularNotification(index++);
                

                var notificationsForGroup = new NotificationsResponce(regularNotification,reminders);
                responceByGroup.Add(group.GroupID,notificationsForGroup);
            }
        }
    }
}