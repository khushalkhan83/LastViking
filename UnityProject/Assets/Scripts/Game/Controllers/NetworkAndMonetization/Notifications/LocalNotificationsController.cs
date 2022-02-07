using UnityEngine;
using Core;
using Core.Controllers;
using Game.Models;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
#if UNITY_IOS
using Unity.Notifications.iOS;
#endif
using System;
using System.Linq;
using NotifData = Game.Models.Notifications.NotificationData;
using Encounters;
using Game.Models.Notifications;
using Game.LocalNotifications;
using System.Text;

namespace Game.Controllers
{
    // TODO: use notification ID instead of constants
    public class LocalNotificationsController : ILocalNotificationsController, IController
    {
        private const string k_ObjectiveKey = "Objective";
        private const string k_WaterfallKey = "Waterfall";
        private const string k_ChurchKey = "Church";
        private const string k_Day3Key = "Day3";
        private const string k_Day7Key = "Day7";
        private const string k_Day28Key = "Day28";

        [Inject] public LocalNotificationsModel model { get; private set; }
        [Inject] public LocalizationModel localization { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public ApplicationCallbacksModel ApplicationCallbacksModel { get; private set; }
        [Inject] public WaterfallProgressModel WaterfallProgressModel { get; private set; }
        [Inject] public ChurchProgressModel ChurchProgressModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public PushNotificationsModel PushNotificationsModel { get; private set; }
        
        public void Enable()
        {
            // InitChanel();
            // DailyNotifications();
            // CancelNotification(k_ObjectiveKey);
            // HandleOpenedGameWithNotification();
            // ApplicationCallbacksModel.ApplicationFocus += OnApplicationFocus;
            // ApplicationCallbacksModel.ApplicationPause += OnApplicationPause;
        }
        public void Start()
        {
            // InitChanel();
        }

        public void Disable()
        {
            // ApplicationCallbacksModel.ApplicationFocus -= OnApplicationFocus;
            // ApplicationCallbacksModel.ApplicationPause -= OnApplicationPause;
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus)
            {
                ObjectiveNotification();
                WaterfallNotification();
                ChurchNotification();
                TrySendGenericNotifications();
            }
            else {
                CancelNotification(k_ObjectiveKey);
                CancelNotification(k_WaterfallKey);
                CancelNotification(k_ChurchKey);
                CancelGenericNotifications();
            }
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                ObjectiveNotification();
                WaterfallNotification();
                ChurchNotification();
                TrySendGenericNotifications();
            }
            else {
                CancelNotification(k_ObjectiveKey);
                CancelNotification(k_WaterfallKey);
                CancelNotification(k_ChurchKey);
                CancelGenericNotifications();
            }
        }

        private void SendNotification(string key,DateTime fireTime)
        {
            LocalNotificationsModel.NotificationSet notificationSet = model.GetSet(key);
            if (notificationSet == null) {
                return;
            }

            SendNotification(notificationSet.ID,fireTime,localization.GetString(notificationSet.title),localization.GetString(notificationSet.text));
        }

        private void SendNotification(int id, DateTime fireTime, string title, string text)
        {
#if UNITY_ANDROID
            var notification = new AndroidNotification();
            notification.Title = title;
            notification.Text = text;
            notification.FireTime = fireTime;
            notification.SmallIcon = model.smallIcon;
            notification.LargeIcon = model.largIcon;

            AndroidNotificationCenter.SendNotificationWithExplicitID(notification, model.chanelID, id);
#endif
#if UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification(id.ToString());

            var timeTrigger = new iOSNotificationTimeIntervalTrigger()
            {
                TimeInterval = fireTime.Subtract(DateTime.Now),
                Repeats = false
            };


            var notification = new iOSNotification()
            {
                Identifier = id.ToString(),
                Title = title,
                Body = text,
                ShowInForeground = true,
                ForegroundPresentationOption = (PresentationOption.Alert | PresentationOption.Sound),
                CategoryIdentifier = model.iOSCategoryIdentifier,
                ThreadIdentifier = model.iOSThreadIdentifier,
                Trigger = timeTrigger,
            };

            iOSNotificationCenter.ScheduleNotification(notification);
#endif
        }

        private void CancelGenericNotifications()
        {
            foreach (IPushNotificationProducer activity in PushNotificationsModel.GetPushNotificationProducers())
            {
                if(!activity.LocalNotificationID.HasValue) continue;

                var notificationsResponce = model.SpecialNotificationsModel.GetNotificationsRequest(activity.LocalNotificationID.Value);

                foreach (var reminder in notificationsResponce.reminders)
                {
                    CancelNotification(reminder.ID);
                }

                CancelNotification(notificationsResponce.notification.ID);
            }
        }

        private void CancelNotification(string key)
        {
            LocalNotificationsModel.NotificationSet notificationSet = model.GetSet(key);
            if (notificationSet == null)
            {
                Debug.LogError("There is no notificatoin with name " + key);
                return;
            }

            CancelNotification(notificationSet.ID);
        }

        void CancelNotification(int id) {
            
#if UNITY_ANDROID
            AndroidNotificationCenter.CancelNotification(id);
#endif
#if UNITY_IOS
            iOSNotificationCenter.RemoveScheduledNotification(id.ToString());
#endif
        }

        private void InitChanel()
        {
#if UNITY_ANDROID
            var c = new AndroidNotificationChannel()
            {
                Id = model.chanelID,
                Name = model.chanelName,
                Importance = model.chanelImportance,
                Description = model.chanelDescriptipn,
            };
            AndroidNotificationCenter.RegisterNotificationChannel(c);
#endif
        }

        private void ObjectiveNotification()
        {
            if (TutorialModel.IsComplete && PlayerObjectivesModel.ReminingTime > model.ObjectiveBeforeEndTimeSec && PlayerObjectivesModel.Pool.Any(o => o.IsHasValue)) 
            {
                DateTime fireTime = DateTime.Now.AddSeconds(PlayerObjectivesModel.ReminingTime - model.ObjectiveBeforeEndTimeSec);
                fireTime = AdjustToDayTime(fireTime);
                SendNotification(k_ObjectiveKey, fireTime);
            }
        }

        private void WaterfallNotification() => DungeonNotification(WaterfallProgressModel);
        private void ChurchNotification() => DungeonNotification(ChurchProgressModel);

        private void DungeonNotification(DungeonProgressModel dungeon)
        {
            if(dungeon.ProgressStatus == Progressables.ProgressStatus.WaitForResetProgress)
            {
                DateTime fireTime = DateTime.Now.AddSeconds(dungeon.GetProgressRemainingTime());
                fireTime = AdjustToDayTime(fireTime);
                SendNotification(k_ChurchKey, fireTime);
            }
        }

        private void DailyNotifications()
        {
            SendNotification(k_Day3Key, DateTime.Now.AddDays(3));
            SendNotification(k_Day7Key, DateTime.Now.AddDays(7));
            SendNotification(k_Day28Key, DateTime.Now.AddDays(28));
        }
        
        private void TrySendGenericNotifications()
        {
            if(!TutorialModel.IsComplete) return;

            foreach (var activity in PushNotificationsModel.GetPushNotificationProducers())
            {
                SendGenericNotifications(activity);
            }
        }

        private void SendGenericNotifications(IPushNotificationProducer activity)
        {
            if (!activity.canSendPushNotifications) return;
            if(!activity.LocalNotificationID.HasValue) return;
            if(activity.isLocked) return;

            var notificationsResponce = model.SpecialNotificationsModel.GetNotificationsRequest(activity.LocalNotificationID.Value);

            DateTime fireTime = DateTime.Now;

            if(activity.cooldownSeconds > 0)
            {
                fireTime = fireTime.AddSeconds(activity.cooldownSeconds);
                fireTime = AdjustToDayTime(fireTime);
                SendGenericNotification(notificationsResponce.notification, fireTime);
            }

            foreach (var reminder in notificationsResponce.reminders)
            {
                fireTime = fireTime.AddSeconds(reminder.WaitInterval);
                fireTime = AdjustToDayTime(fireTime);
                SendGenericNotification(reminder, fireTime);
            }
        }

        private void SendGenericNotification(NotifData data, DateTime time)
        {
            SendNotification(data.ID, time, localization.GetString(data.Title), localization.GetString(data.Text));
        }

        private DateTime AdjustToDayTime(DateTime fireTime)
        {
            if(fireTime.Hour >= model.MaxEveningHours)
            {
                return fireTime.AddDays(1).Date + new TimeSpan(model.MinMorningHours, 0, 0);
            }
            else if(fireTime.Hour < model.MinMorningHours)
            {
                return fireTime.Date + new TimeSpan(model.MinMorningHours, 0, 0);
            }
            else
            {
                return fireTime;
            }
        }

        private void HandleOpenedGameWithNotification()
        {
            if(!TryGetNotificationIDThatOpenedGame(out int id)) return;

            var notificationData = model.GetSetById(id);

            if(notificationData != null)
            {
                model.GameStartedFromNotification(notificationData.notificationID);
            }
            else
            {
                if(model.SpecialNotificationsModel.TryGetLocalNotifiicationID(id, out var notificationID))
                {
                    model.GameStartedFromNotification(notificationID);
                }
            }
        }

        private bool TryGetNotificationIDThatOpenedGame(out int id)
        {
            id = 0;
            #if UNITY_ANDROID
            var androidNotificationData = AndroidNotificationCenter.GetLastNotificationIntent();
            if(androidNotificationData == null)
            {
                return false;
            }
            else
            {
                id = androidNotificationData.Id;
                return true;
            }
            #endif

            #if UNITY_IOS
            var iosNotificationData = iOSNotificationCenter.GetLastRespondedNotification();
            // LogInfoAboutNotification(iosNotificationData);
            if(iosNotificationData == null)
            {
                return false;
            }
            else
            {
                bool ok = int.TryParse(iosNotificationData.Identifier, out var result);
                if(!ok) return false;

                id = result;
                return true;
            }
            #endif

            return false;
        }

        #if UNITY_IOS

        private void LogInfoAboutNotification(iOSNotification iosNotificationData)
        {
            if(iosNotificationData == null)
            {
                Debug.Log("No notification data");
                return;
            }

            var sb = new StringBuilder();
                Debug.Log("iosNotificationData.Identifier" + iosNotificationData.Identifier);
                Debug.Log("iosNotificationData.Title" + iosNotificationData.Title);
                Debug.Log("iosNotificationData.Subtitle" + iosNotificationData.Subtitle);
                Debug.Log("iosNotificationData.Badge" + iosNotificationData.Badge);
                Debug.Log("iosNotificationData.Body" + iosNotificationData.Body);
                Debug.Log("iosNotificationData.CategoryIdentifier" + iosNotificationData.CategoryIdentifier);
                Debug.Log("iosNotificationData.Data" + iosNotificationData.Data);

            Debug.Log(sb.ToString());
        }

        #endif
    }
}