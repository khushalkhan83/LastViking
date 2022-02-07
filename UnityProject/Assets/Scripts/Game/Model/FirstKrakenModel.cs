using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using UnityEngine;
using SO_Variables_General;
using Game.AI.Behaviours.Kraken;

namespace Game.Models
{
    public class FirstKrakenModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredBool Active;
            public ObscuredBool WasInited;
            public ObscuredFloat Health;
            public ObscuredFloat MaxHealth;

            public void SetActive(bool value)
            {
                Active = value;
                ChangeData();
            }

            public void SetWasInited(bool value)
            {
                WasInited = value;
                ChangeData();
            }

            public void SetHealth(float value)
            {
                Health = value;
                ChangeData();
            }

            public void SetMaxHealth(float value)
            {
                MaxHealth = value;
                ChangeData();
            }
        }
        [SerializeField] private KrakenConfig _krakenConfig;

        [SerializeField] private Data _data;
        public Data FirstKrakenData => _data;
        public bool WasInited => _data.WasInited;
        public bool Active => _data.Active;
        public float Health => _data.Health;
        public float MaxHealth => _data.MaxHealth;
        public float HealthOnStart => _krakenConfig.HealthOnStart;
        public bool RunAway => _krakenConfig.RunAway;
        public float HealthToRunAway => _krakenConfig.HealthToRunAway;

        public event Action OnKrakenPreActivate;
        public event Action OnKrakenActivate;
        public event Action OnKrakenWasKilled;
        public event Action OnKrakenSpawned;

        public event Action<float> OnKrakenHealthChanged;


        public void SetKrakenActive()
        {
            _data.SetWasInited(true);
            _data.SetActive(true);
            OnKrakenPreActivate?.Invoke();
            OnKrakenActivate?.Invoke();
        }

        public void SetKrakenKilled()
        {
            _data.SetActive(false);
            OnKrakenWasKilled?.Invoke();
        }

        public void SetKrakenSpawned()
        {
            OnKrakenSpawned?.Invoke();
        }

        public void SetKrakenHealth(float value)
        {
            _data.SetHealth(value);
            OnKrakenHealthChanged?.Invoke(value);
        }

        public void SetKrakenMaxHealth(float value)
        {
            _data.SetMaxHealth(value);
        }

        public void ResetData()
        {
            _data.SetActive(false);
            _data.SetWasInited(false);
        }

        public void SetConfig(KrakenConfig config)
        {
            _krakenConfig = config;
        }

        public KrakenConfig Config => _krakenConfig;
    }
}
