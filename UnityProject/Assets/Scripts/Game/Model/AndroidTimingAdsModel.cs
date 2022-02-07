using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class AndroidTimingAdsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredBool HasShown;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

        [SerializeField] private float _waitTime = 60;
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

        public bool CanShow { set; get; }

        public bool HasShown
        {
            get
            {
                return _data.HasShown;
            }
            private set
            {
                _data.HasShown = value;
            }
        }
        
        public void ShowAds()
        {
            HasShown = true;
        }
    }
}
