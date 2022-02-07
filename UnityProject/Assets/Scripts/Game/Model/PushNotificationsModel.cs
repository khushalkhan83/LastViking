using System.Collections.Generic;
using System.Linq;
using Game.LocalNotifications;
using Game.Models;
using NaughtyAttributes;
using UnityEngine;

namespace Game.LocalNotifications
{
    public interface IPushNotificationProducer
    {
        LocalNotificationID? LocalNotificationID {get;}
        bool canSendPushNotifications {get;}
        float cooldownSeconds {get;}
        bool isLocked {get;}
    }
}

namespace Game.Models.Notifications
{

    public class PushNotificationsModel : MonoBehaviour
    {
        #region Dependencies
        private EncountersModel EncountersModel => ModelsSystem.Instance._encountersModel;
        // private DungeonsProgressModel DungeonsProgressModel => ModelsSystem.Instance._dungeonsProgressModel;
            
        #endregion

        #region MonoBehaviour
        private void OnEnable()
        {
            activities = null;
        }
        #endregion

        private List<IPushNotificationProducer> activities;

        public List<IPushNotificationProducer> GetPushNotificationProducers()
        {
            if(activities == null)
            {
                activities = new List<IPushNotificationProducer>();

                List<IPushNotificationProducer> encounters = EncountersModel.GetPushNotificationProducers().ToList();
                // List<IRepeatableActivity> dungeons = DungeonsProgressModel.GetPushNotificationProducers();
                
                activities.AddRange(encounters);
            }
            
            return activities;
        }


        [Button] void Test()
        {
            var answer = GetPushNotificationProducers();
        }
    }
}