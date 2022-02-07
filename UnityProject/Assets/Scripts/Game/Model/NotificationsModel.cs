using System;
using System.Collections.Generic;
using System.Linq;
using Game.InGameNotifications;
using UnityEngine;

namespace Game.Models
{
    public class NotificationsModel : MonoBehaviour
    {
        public event Action OnNotificationDataAdded;
        public Queue<InGameNotificationData> notificationDatas = new Queue<InGameNotificationData>();
        public void SendNotification(InGameNotificationData notificationData, bool highPriority = false)
        {
            if(notificationDatas == null) return;

            if (IsDublicateMessage(notificationData)) return;

            if (highPriority)
            {
                var items = notificationDatas.ToArray();
                notificationDatas.Clear();
                notificationDatas.Enqueue(notificationData);

                foreach (var item in items)
                {
                    notificationDatas.Enqueue(item);
                }
            }
            else
            {
                notificationDatas.Enqueue(notificationData);
            }

            OnNotificationDataAdded?.Invoke();
        }

        private bool IsDublicateMessage(InGameNotificationData notificationData)
        {
            return notificationDatas.ToList().Find(x => x.message == notificationData.message) != null;
        }
    }
}

namespace Game.InGameNotifications
{
    public class InGameNotificationData
    {
        public readonly Func<string> message;
        public readonly Sprite icon;

        public InGameNotificationData(Func<string> message, Sprite icon)
        {
            this.message = message;
            this.icon = icon;
        }
    }
}
