using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class MoreGamesModel : InitableModel<MoreGamesModel.Data>
    {
        [Serializable]
        public class Data: DataBase
        {
            [SerializeField] private bool isLastFishRewardReceived;

            public bool IsLastRewardReceived
            {
                get { return isLastFishRewardReceived; }
                set { isLastFishRewardReceived = value; ChangeData(); }
            }
        }


        [SerializeField] private Data data = default;
        [SerializeField] private string googlePlayLastFishLink = "market://details?id=com.RetrostyleGames.LastFisherman.sea.pirate";
        [SerializeField] private string appStoreLastFishLink = default;
        [SerializeField] private float rewardPauseTime = 10f;


        public event Action OnLastFishLinkClicked;
        public event Action OnLastRewardReceivedChanged;

        protected override Data DataBase => data;

        public bool IsLastFishRewardReceived
        {
            get{return data.IsLastRewardReceived;}
            private set{data.IsLastRewardReceived = value;}
        }

        public bool PlatformSuportedFishingLink
        {
            get
            {
            #if UNITY_ANDROID && !AMAZON_STORE
                return true;
            #else
                return false;
            #endif
            }
        }

        public string LastFishLink
        {
            get
            {
            #if UNITY_ANDROID && !AMAZON_STORE
                return googlePlayLastFishLink;
            #else
                return appStoreLastFishLink;
            #endif
            }
        }

        public float RewardPauseTime => rewardPauseTime;

        public void ClickLastFishLink() => OnLastFishLinkClicked?.Invoke();

        public void SetLastFishRewardRecieved(bool isReceived)
        {
            IsLastFishRewardReceived = isReceived;
            OnLastRewardReceivedChanged?.Invoke();
        }
        
    }
}
