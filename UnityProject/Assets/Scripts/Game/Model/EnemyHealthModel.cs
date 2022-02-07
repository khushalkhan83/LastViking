using Core.Storage;
using Game.AI.BehaviorDesigner;
using Game.AI.Behaviours.Kraken;
using Game.ObjectPooling;
using NaughtyAttributes;
using SOArchitecture;
using System;
using UnityEngine;

namespace Game.Models
{
    public class EnemyHealthModel : MonoBehaviour, IHealth, IResettable, IKrakenConfigurable
    {
        [Serializable]
        public class Data : DataBase
        {
            public float Health;

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
        [SerializeField] private bool _useConfig;
        [ShowIf("_useConfig")]
        [SerializeField] private bool _useKrakenConfig;
        [ShowIf("_useConfig")]
        [SerializeField] private EnemyConfig _config;
        [ShowIf("_useConfig")]
        [SerializeField] private KrakenConfig _krakenConfig;
        [SerializeField] private GameEvent _gameEventOnDeath;

#pragma warning restore 0649
        #endregion

        #region Dependencies

        private EnemiesModifierModel EnemiesModifierModel => ModelsSystem.Instance._enemiesModifierModel;
            
        #endregion

        private float? MaxHealthFromConfig
        {
            get
            {
                if(!_useConfig) return null;
                if(_useKrakenConfig)
                {
                    if(_krakenConfig == null) return null;
                    return _krakenConfig.MaxHealth;
                }
                else
                {
                    if(_config == null) return null;
                    return _config.Health;
                }
            }
        }

        #region IHealth
            
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
        public float HealthMax => (MaxHealthFromConfig?? _healthMax) * EnemiesModifierModel.HealthScaler;
        public bool IsDead => Health <= 0;
        public event Action OnChangeHealth;
        public event Action OnDeath;

        public void AdjustHealth(float adjustment)
        {
            Health += adjustment;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                OnDeath?.Invoke();
                _gameEventOnDeath?.Raise();
            }
        }

        public void SetHealth(float health)
        {
            Health = health;
            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                OnDeath?.Invoke();
                _gameEventOnDeath?.Raise();
            }
        }
        #endregion

        #region IResettable
        public void ResetObject()
        {
            SetHealth(HealthMax);
        }
        #endregion

        #region IKrakenConfigurable
        public void Configurate(KrakenConfig config)
        {
            if(_useConfig == false || _useKrakenConfig == false)
            {
                Debug.LogError("Can`t configure health. Check settings");
                return;
            }
            _krakenConfig = config;
        }
            
        #endregion
    }
}
