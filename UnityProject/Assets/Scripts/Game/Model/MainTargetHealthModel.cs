using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class MainTargetHealthModel : MonoBehaviour, IHealth
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat Health;

            public void SetHealth (float health)
            {
                Health = health;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ObscuredFloat _healthMax;

#pragma warning restore 0649
        #endregion

        public float Health
        {
            get
            {
                return _data.Health;
            }
            protected set
            {
                _data.SetHealth(value);
            }
        }

        public float HealthMax => _healthMax;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public bool IsDead => Health <= 0;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public event Action OnChangeHealth;
        public event Action OnDeath;

        public void AdjustHealth(float adjustment)
        {
            Health += adjustment;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void SetHealth(float health)
        {
            Health = health;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }
    }
}
