using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class TimingAdsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredBool HasShown;

            public void SetHasShown(bool hasShown)
            {
                HasShown = hasShown;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

        [SerializeField] private float _waitTime = 180;
        [SerializeField] private float _loadAdsGapTime = 20;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => _storageModel;

        public float WaitTime => _waitTime;
        public float LoadAdsGapTime => _loadAdsGapTime;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public bool IsCanShow { get; private set; }

        public bool HasShown
        {
            get => _data.HasShown;
            private set => _data.SetHasShown(value);
        }

        public void ShowAds()
        {
            HasShown = true;
        }

        public void SetCanShow(bool show)
        {
            IsCanShow = show;
        }
    }
}
