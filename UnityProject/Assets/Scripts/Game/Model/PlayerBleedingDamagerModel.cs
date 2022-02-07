using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerBleedingDamagerModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public bool IsHitFromEnemy;

            public void SetBleeding(bool isHitFromEnemy)
            {
                IsHitFromEnemy = isHitFromEnemy;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;
        [SerializeField] private float _damageTimeDelay;
        [SerializeField] private float _damageTimeDelayDefault;
        [SerializeField] private float _damage;
        [SerializeField] private float _lowerHealthThreshold;

#pragma warning restore 0649
        #endregion

        public bool IsBleeding
        {
            get => _data.IsHitFromEnemy;
            private set => _data.SetBleeding(value);
        }

        public bool IsInitializeData { get; private set; }

        public Data _Data => _data;
        public StorageModel StorageModel => _storageModel;
        public float DamageTimeDelay => _damageTimeDelay;
        public float DamageTimeDelayDefault => _damageTimeDelayDefault;
        public float Damage => _damage;
        public float LowerHealthThreshold => _lowerHealthThreshold;

        public event Action OnLoadData;
        public event Action OnChangeHitFrom;

        public void SetBleeding(bool value)
        {
            IsBleeding = value;
            OnChangeHitFrom?.Invoke();
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
            InitializeData();
        }

        public void InitializeData()
        {
            IsInitializeData = true;
            OnLoadData?.Invoke();
        }

        public void SetTimeDelayDefault() => _damageTimeDelay = _damageTimeDelayDefault;
        public void AdjustTimeDelay(float value) => _damageTimeDelay += value;
    }
}
