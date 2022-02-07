using UnityEngine;
using System;
using RoboRyanTron.SearchableEnum;
using System.Collections.Generic;

namespace Game.Models.Notifications
{
    [Serializable]
    public class NotificationGroup
    {
        #region Data
        #pragma warning disable 0649
        
        [SearchableEnum] [SerializeField]  private LocalizationKeyID _title, _text;
        [SerializeField] private List<int> _remindIntervals;
        [SearchableEnum] [SerializeField] private LocalNotificationID _localNotificationID;
        #pragma warning restore 0649
        #endregion
        
        public int NotificationsCount => _remindIntervals.Count;
        public LocalNotificationID GroupID => _localNotificationID;

        public IEnumerable<NotificationData> GetReminders(int startIndex)
        {
            var answer = new List<NotificationData>();

            for (int i = 0; i < _remindIntervals.Count; i++)
            {
                var waitInterval = _remindIntervals[i];
                var notification = new NotificationData(_title,_text,_localNotificationID,startIndex + i,waitInterval);
                answer.Add(notification);
            }

            return answer;
        }

        public NotificationData GetRegularNotification(int index)
        {
            var notification = new NotificationData(_title,_text,_localNotificationID,index,0);
            return notification;
        }
    }
}