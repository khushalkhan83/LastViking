using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class RateMeViewModel : InitableModel<RateMeViewModel.Data>
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public bool IsCanShow;

            public void SetIsCanShow(bool isCanShow)
            {
                IsCanShow = isCanShow;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredString _urlAndroid;
        [SerializeField] private ObscuredString _urlITunes;
        [SerializeField] private Data _data;
        [SerializeField] private int _rateShowDeathCount;
        [SerializeField] private float _delayBeforeGetCoins;
        [SerializeField] private float _delayBeforeShowView;

#pragma warning restore 0649
        #endregion

        public string URLAndroid => _urlAndroid;
        public string URLITunes => _urlITunes;
        public int RateShowDeathCount => _rateShowDeathCount;
        public float DelayBeforeGetCoins => _delayBeforeGetCoins;
        public float DelayBeforeShowView => _delayBeforeShowView;
        protected override Data DataBase => _data;

        public string URL =>
#if UNITY_ANDROID
            URLAndroid;
#elif UNITY_IPHONE
            URLITunes;
#endif

        public bool IsCanShow
        {
            get
            {
                return _data.IsCanShow;
            }
            private set
            {
                _data.SetIsCanShow(value);
            }
        }

        public event Action OnChangeIsCanShow;

        public void AlwaysHide()
        {
            IsCanShow = false;
            OnChangeIsCanShow?.Invoke();
        }
    }
}
