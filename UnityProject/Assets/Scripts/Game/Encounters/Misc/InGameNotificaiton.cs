using System;
using Game.Data;
using Game.Models;
using UnityEngine;

namespace Encounters
{
    public class ScreenTopNotification : IInGameNotification
    {
        private Func<string> Message {get;}
        private readonly Sprite icon;

        private NotificationsModel NotificationsModel => ModelsSystem.Instance._notificationsModel;


        public ScreenTopNotification(Func<string> message, Sprite icon)
        {
            this.Message = message;
            this.icon = icon;
        }
        public ScreenTopNotification(Game.InGameNotifications.InGameNotificationData data)
        {
            this.Message = data.message;
            this.icon = data.icon;
        }

        public void Show()
        {
            NotificationsModel.SendNotification(new Game.InGameNotifications.InGameNotificationData(Message, icon));
        }
    }
    public class SpecialInGameNotificaiton : IInGameNotification
    {
        private readonly string message;
        public SpesialMessagesModel SpesialMessagesModel => ModelsSystem.Instance._spesialMessagesModel;
        public ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;


        public SpecialInGameNotificaiton(string message)
        {
            this.message = message;
        }

        public void Show()
        {
            RecivedItemMessageData data = new RecivedItemMessageData(ItemsDB.GetItem("tool_fishing_rod").Icon,
                                                                        message,
                                                                        string.Empty);
            SpesialMessagesModel.RecivedItem(data);
        }
    }
}