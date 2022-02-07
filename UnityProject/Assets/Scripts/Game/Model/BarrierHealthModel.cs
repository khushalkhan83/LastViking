using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class BarrierHealthModel : MonoBehaviour, IHealth
    {
        [Serializable]
        public class Data : DataBase
        {
            public float Health;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetHealth(float health)
            {
                Health = health;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private float _healthMax;

#pragma warning restore 0649
        #endregion

        public float Health
        {
            get => _data.Health;
            protected set => _data.SetHealth(value);
        }

        public float HealthMax => _healthMax;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public bool IsDead => Health <= 0;

        public event Action OnChangeHealth;
        public event Action OnDeath;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public void AdjustHealth(float adjustment)
        {
            Health += adjustment;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                Death();
            }
        }

        public void Death()
        {
            OnDeath?.Invoke();
        }

        public void SetHealth(float health)
        {
            Health = health;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                Death();
            }
        }
    }
}
