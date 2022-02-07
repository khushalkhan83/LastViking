using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerWarmModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat warm = 100f;

            public void SetWarm(float value)
            {
                warm = value;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private float _maxWarm = 100f;
        [SerializeField] private float _coldPerSecond;
        [SerializeField] private float _warmPerSecond;
        [SerializeField] private float _warmToFreeze;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        public float MaxWarm => _maxWarm;
        public float ColdPerSecond => _coldPerSecond;
        public float WarmPerSecond => _warmPerSecond;
        public float WarmToFreeze => _warmToFreeze;

        public float Warm
        {
            get => _data.warm;
            private set => _data.SetWarm(value);
        }

        public bool IsColding {get; private set;}
        public bool IsInColdZone {get; private set;}
        public bool IsInWarmZone {get; private set;}

        public event Action OnChangeWarm;
        public event Action OnStartColding;
        public event Action OnStopColding;

        private bool isInColdZoneUpdated;
        private bool isInWarmZoneUpdated;

        public void StartColding()
        {
            if(!IsColding)
            {
                IsColding = true;
                OnStartColding?.Invoke();
            }
        }

        public void StopColding()
        {
            if(IsColding)
            {
                IsColding = false;
                OnStopColding?.Invoke();
            }
        }

        public void AdjustWarm(float value)
        {
            Warm = Mathf.Clamp(Warm + value, 0, _maxWarm);
            OnChangeWarm?.Invoke();
        }

        public void SetInColdZone()
        {
            IsInColdZone = true;
            isInColdZoneUpdated = true;
        }

        public void SetInWarmZone()
        {
            IsInWarmZone = true;
            isInWarmZoneUpdated = true;
        }

        private void FixedUpdate() 
        {
            if(isInColdZoneUpdated)
                isInColdZoneUpdated = false;
            else
                IsInColdZone = false;

            if(isInWarmZoneUpdated)
                isInWarmZoneUpdated = false;
            else
                IsInWarmZone = false;
        }
    }
}
