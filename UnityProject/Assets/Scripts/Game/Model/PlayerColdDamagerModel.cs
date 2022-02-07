using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerColdDamagerModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat RemainingTimeHealthDecrease;

            public void SetRemainingTimeHealthDecrease(float remainingTimeHealthDecrease)
            {
                RemainingTimeHealthDecrease = remainingTimeHealthDecrease;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private ObscuredFloat _timeHealthDecrease;
        [SerializeField] private ObscuredFloat _healthAdjustment;

#pragma warning restore 0649
        #endregion

        public float RemainingTimeHealthDecrease
        {
            get => _data.RemainingTimeHealthDecrease;
            private set => _data.SetRemainingTimeHealthDecrease(value);
        }

        public Data _Data => _data;

        public float TimeHealthDecrease => _timeHealthDecrease;
        public float HealthAdjustment => _healthAdjustment;

        public event Action OnHealthDecrease;

        public void SetRemainingTimeHealthDecrease(float ticks) => RemainingTimeHealthDecrease = ticks;

        public void HealthDecrease() => OnHealthDecrease?.Invoke();
    }
}
