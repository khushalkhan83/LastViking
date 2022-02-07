using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using NaughtyAttributes;
using RoboRyanTron.SearchableEnum;
using Game.Models.Notifications;
#if UNITY_ANDROID
using Unity.Notifications.Android;
#endif
using Core.Storage;

namespace Game.Models
{
    public enum LocalNotificationID
    {
        None = 0,
        Day3 = 10,
        Day7 = 11,
        Day28 = 12,
        Objective = 20,
        Waterfall = 30,
        Church = 31,
        SpecialEncounter_crab = 50,
        SpecialEncounter_chestCrab = 51,
    }
    
    // TODO: Refactor (use NotificationID instead of name, delete unsed code)
    public class LocalNotificationsModel : MonoBehaviour
    {

        [System.Serializable]
        public class NotificationsSave : DataBase, IImmortal
        {
            [SerializeField]
            Dictionary<string, int> notificationIDs;

            public int GetNotificationID(string key)
            {
                if (notificationIDs == null)
                    return -1;
                if (notificationIDs.ContainsKey(key))
                    return notificationIDs[key];
                return -1;
            }

            public void SetNotificationID(string key,int newID)
            {
                if (notificationIDs == null)
                    notificationIDs = new Dictionary<string, int>();
                if (notificationIDs.ContainsKey (key))
                {
                    if (notificationIDs[key] != newID)
                    {
                        notificationIDs[key] = newID;
                        ChangeData();
                    }
                }
                else
                {
                    notificationIDs.Add(key, newID);
                    ChangeData();
                }                
            }
        }

        [System.Serializable]
        public class NotificationSet
        {
            [SerializeField] string _name;
            [SerializeField] int _id;
            [SearchableEnum]
            [SerializeField] LocalizationKeyID _title, _text;
            [SerializeField] int _wait = 60;
            [SearchableEnum]
            [SerializeField] private LocalNotificationID _notificationID;

            public string name => _name;
            public int ID => _id;
            public LocalizationKeyID title => _title;
            public LocalizationKeyID text => _text;
            public int secondsWait => _wait;
            public LocalNotificationID notificationID => _notificationID;
        }

        #region Config
        [SerializeField] private SpecialNotificationsModel _specialNotificationsModel;

        [SerializeField]
        NotificationSet[] _notifications;
        [SerializeField]
        string _chanelID = "last_pirate";
        [SerializeField]
        string _chanelName = "Last Pirate";
        [SerializeField]
        string _chanelDescriptoin = "Last Pirate's Local Notifications";
#if UNITY_ANDROID
        [SerializeField]
        Importance _chanelImportance = Importance.High;
#endif
        [SerializeField]
        string _smallIcon = "icon_small";
        [SerializeField]
        string _largeIcon = "icon_large";
        [SerializeField]
        string _iOSCategoryIdentifier = "last_pirate";
        [SerializeField]
        string _iOSThreadIdentifier = "last_pirate";
        [SerializeField]
        NotificationsSave _identificators;
        [SerializeField]
        float _objectiveBeforeEndTimeSec = 3600;
        [SerializeField] private int _minMorningHours = 9;
        [SerializeField] private int _maxEveningHours = 23;
        #endregion

        public string chanelID => _chanelID;
        public string chanelName => _chanelName;
        public string chanelDescriptipn => _chanelDescriptoin;
#if UNITY_ANDROID
        public Importance chanelImportance => _chanelImportance;
#endif
        public string smallIcon => _smallIcon;
        public string largIcon => _largeIcon;
        public string iOSCategoryIdentifier => _iOSCategoryIdentifier;
        public string iOSThreadIdentifier => _iOSThreadIdentifier;
        public float ObjectiveBeforeEndTimeSec => _objectiveBeforeEndTimeSec;
        public int MinMorningHours => _minMorningHours;
        public int MaxEveningHours => _maxEveningHours;

        public SpecialNotificationsModel SpecialNotificationsModel => _specialNotificationsModel;

        public int GetNotificationID(string key)
        {
            return _identificators.GetNotificationID(key);
        }

        public void SetNotificationID(string key,int newID) => _identificators.SetNotificationID(key, newID);

        public bool GetTitle(string key, out LocalizationKeyID locID)
        {
            NotificationSet set = GetSet(key);
            if (set!=null)
            {
                locID = set.title;
                return true;
            }
            else
            {
                locID = LocalizationKeyID.None;
                return false;
            }
        }

        public NotificationSet GetSet(string key)
        {
            foreach (NotificationSet set in _notifications)
            {
                if (set.name.Equals(key))
                {
                    return set;
                }
            }
            return null;
        }

        public NotificationSet GetSetById(int id)
        {
            foreach (NotificationSet set in _notifications)
            {
                if (set.ID.Equals(id))
                {
                    return set;
                }
            }
            return null;
        }

        public bool GetText(string key, out LocalizationKeyID locID)
        {
            NotificationSet set = GetSet(key);
            if (set != null)
            {
                locID = set.text;
                return true;
            }
            else
            {
                locID = LocalizationKeyID.None;
                return false;
            }
        }
        
        public event Action<LocalNotificationID> OnGameStartedFromNotification;

        public LocalNotificationID? GameOpenedWithNotification;

        [SerializeField] private LocalNotificationID testNoficationID;
        [Button] void Test() => GameStartedFromNotification(testNoficationID);
        
        public void GameStartedFromNotification(LocalNotificationID notificationID)
        {
            GameOpenedWithNotification = notificationID;
            OnGameStartedFromNotification?.Invoke(notificationID);
        }
    }
}