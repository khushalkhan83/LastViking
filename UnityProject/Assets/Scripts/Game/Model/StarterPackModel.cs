using Core.Storage;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using Game.Views;

namespace Game.Models
{
    public class StarterPackModel : MonoBehaviour
    {
        public enum OfferStatus
        {
            WaitForAvailable,
            Available,
            Expired,
            Bought
        }

        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public OfferStatus OfferStatus;
            public long EndOfferTime;
            public bool HasRepeatedShown;
            public bool NextSessionShown;

            public void SetOfferStatus(OfferStatus status)
            {
                OfferStatus = status;
                ChangeData();
            }

            public void SetEndOfferTime(long time)
            {
                EndOfferTime = time;
                ChangeData();
            }

            public void SetHasRepeatedShown(bool shown)
            {
                HasRepeatedShown = shown;
                ChangeData();
            }

            public void SetNextSessionShown(bool shown)
            {
                NextSessionShown = shown;
                ChangeData();
            }

        }

        [Serializable]
        public class ItemSettings
        {
            [SerializeField] private string itemName;
            [SerializeField] private int count;

            public string ItemName => itemName;
            public int Count => count;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private float _offerDuration;
        [SerializeField] private int _repeatedOfferDay;
        [SerializeField] private int _beforeEndTimeSec = 300;
        [SerializeField] private ItemSettings[] _survivalItems;
        [SerializeField] private ItemSettings[] _dominationItems;
        [SerializeField] private List<ViewID> _canShowOverViews;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        public float OfferDuration => _offerDuration;
        public ItemSettings[] SurvivalItems => _survivalItems;
        public ItemSettings[] DominationItems => _dominationItems;
        public List<ViewID> CanShowOverViews => _canShowOverViews;

        public event Action OnOfferEnded;
        public event Action OnPackBought;

        #region Starter Pack Events
            
        public event Action OnShowStarterPackPopupFromIcon;
        public event Action OnShowStarterPackPopupFirstTime;
        public event Action OnShowStarterPackPopupSecondTime;
        public event Action OnShowStarterPackPopupFromLocalNotif;
        #endregion

        public OfferStatus CurrentOfferStatus
        {
            get => _Data.OfferStatus;
            set => _Data.SetOfferStatus(value);
        }

        public long EndOfferTime
        {
            get => _Data.EndOfferTime;
            set => _Data.SetEndOfferTime(value);
        }

        public bool HasRepeatedShown
        {
            get => _Data.HasRepeatedShown;
            set => _Data.SetHasRepeatedShown(value);
        }

        public bool NextSessionShown {get;set;}

        public float RemainOfferTime { get; private set; }
        public int RepeatedOfferDay => _repeatedOfferDay;
        public int BeforeEndTimeSec => _beforeEndTimeSec;

        public bool IsPackAvailable => CurrentOfferStatus == OfferStatus.Available || CurrentOfferStatus == OfferStatus.WaitForAvailable;

        public void SetRemainOfferTime(float remainTime) => RemainOfferTime = remainTime;

        public void UpdateTime(float deltaTime)
        {
            RemainOfferTime -= deltaTime;
            if (RemainOfferTime <= 0)
            {
                OfferEnded();
            }
        }

        public void BuyPack()
        {
            CurrentOfferStatus = OfferStatus.Bought;
            OnPackBought?.Invoke();
        }

        #region Starter Pack Related
            
        public void ShowStarterPackPopupFromIcon() => OnShowStarterPackPopupFromIcon?.Invoke();
        public void ShowStarterPackPopupFirstTime()
        {
            CurrentOfferStatus = OfferStatus.Available;
            OnShowStarterPackPopupFirstTime?.Invoke();
        }
        public void ShowStarterPackPopupSecondTime() => OnShowStarterPackPopupSecondTime?.Invoke();
        public void ShowStarterPackPopupNextSession()
        { 
            NextSessionShown = true;
            OnShowStarterPackPopupSecondTime?.Invoke();
        }
        // TODO: add support
        public void ShowStarterPackPopupFromLocalNotif() => OnShowStarterPackPopupFromLocalNotif?.Invoke();

        #endregion

        private void OfferEnded()
        {
            CurrentOfferStatus = OfferStatus.Expired;
            OnOfferEnded?.Invoke();
        }
    }
}
