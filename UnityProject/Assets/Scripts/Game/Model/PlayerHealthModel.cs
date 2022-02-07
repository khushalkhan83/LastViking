using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerHealthModel : MonoBehaviour, IHealth
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat Health;
            public ObscuredFloat HealthAddon;
            public ObscuredInt AddonLevel;

            public void SetHealth(float health)
            {
                Health = health;
                ChangeData();
            }

            public void SetHealthAddon(float healthAddon)
            {
                HealthAddon = healthAddon;
                ChangeData();
            }

            public void SetAddonLevel(int addonLevel)
            {
                AddonLevel = addonLevel;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ObscuredFloat _healthMax;
        [SerializeField] private ObscuredFloat[] _addonLevels;
        [SerializeField] private ObscuredFloat _healthBonus;


#pragma warning restore 0649
        #endregion

        public float Health
        {
            get
            {
                return _data.Health;
            }
            private set
            {
                _data.SetHealth(value);
            }
        }

        public float HealthAddon
        {
            get
            {
                return _data.HealthAddon;
            }
            private set
            {
                _data.SetHealthAddon(value);
            }
        }

        public int AddonLevel
        {
            get
            {
                return _data.AddonLevel;
            }
            protected set
            {
                _data.SetAddonLevel(value);
            }
        }

        public float EquipmentBonus { get; private set;}

        public ObscuredFloat[] AddonLevels => _addonLevels;
        public float HealthCurrent => Health + HealthAddon;
        public float HealthCurrentMax => HealthCurrent > HealthMax ? HealthCurrent : HealthMax;
        public float HealthMax => _healthMax + EquipmentBonus;
        public float HealthAddonMax => GetAddonMax(AddonLevel);
        public float HealthAllMax => HealthMax + HealthAddonMax + EquipmentBonus;
        public float HealthBonus => _healthBonus;

        public bool IsDead => HealthCurrent == 0;

        public event Action OnDeath;
        public event Action OnChangeHealth;
        public event Action OnAddHealth;
        public event Action OnChangeAddonLevel;
        public event Action OnChangeEquipmentBonus;

        public bool IsCantRecieveDamage { get; private set; }

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public float GetAddonMax(int level)
        {
            if (level < AddonLevels.Length)
            {
                return AddonLevels[level];
            }

            return AddonLevels[AddonLevels.Length - 1];
        }

        public void CantRecieveDamage(bool isCantRecieveDamage)
        {
            IsCantRecieveDamage = isCantRecieveDamage;
        }

        public void SetAddonLevel(int level)
        {
            AddonLevel = level;
            OnChangeAddonLevel?.Invoke();
        }

        public void AddAddon()
        {
            if (!IsCantRecieveDamage)
            { 
                var health = Health;
                var left = AdjustValue(ref health, HealthAddonMax, 0, HealthMax);
                Health = health;

                if (left > 0)
                {
                    var healthAddon = HealthAddon;
                    AdjustValue(ref healthAddon, left, 0, HealthAddonMax);
                    HealthAddon = healthAddon;
                }

                OnChangeHealth?.Invoke();
                OnAddHealth?.Invoke();

                if (IsDead)
                {
                    OnDeath?.Invoke();
                }
            }
        }

        public void AddBonus()
        {
            if (!IsCantRecieveDamage)
            {
                var health = Health;
                var left = AdjustValue(ref health, HealthBonus, 0, HealthMax);
                Health = health;

                if (left > 0)
                {
                    var healthAddon = HealthAddon;
                    AdjustValue(ref healthAddon, left, 0, HealthBonus);
                    HealthAddon = healthAddon;
                }

                OnChangeHealth?.Invoke();
                OnAddHealth?.Invoke();
            }
        }

        public void SetEquipmentBonus(float bonus)
        {
            EquipmentBonus = bonus;

            if(Health > HealthMax) // Set health to max value after equipment bonuse reduced and health biger than max health
            {
                Health = HealthMax;
                OnChangeHealth?.Invoke();
            }
            else if(Health < HealthMax && HealthAddon > 0) // Move health points from HealthAddon to Health when equipment bonuse increased
            {
                float needHealthToMax = HealthMax - Health;

                var health = Health;
                var healthAddon = HealthAddon;

                var left = AdjustValue(ref healthAddon, -needHealthToMax, 0, HealthBonus);

                float addToHealth = needHealthToMax + left;
                AdjustValue(ref health, addToHealth, 0, HealthMax);

                HealthAddon = healthAddon;
                Health = health;

                OnChangeHealth?.Invoke();
            }

            OnChangeEquipmentBonus?.Invoke();
        }

        public void AdjustHealth(float adjustment)
        {
            if (!IsCantRecieveDamage)
            {
                if (adjustment < 0)
                {
                    AdjustSubHealth(adjustment);
                }
                else
                {
                    AdjustAddHealth(adjustment);
                }

                OnChangeHealth?.Invoke();
                if (adjustment > 0)
                    OnAddHealth?.Invoke();

                if (IsDead)
                {
                    OnDeath?.Invoke();
                }
            }
        }

        public void AdjustHealth(float adjustment, bool isOverflow)
        {
            if (!IsCantRecieveDamage)
            {
                if (adjustment < 0)
                {
                    AdjustSubHealth(adjustment);
                }
                else
                {
                    if (isOverflow && Health + adjustment > HealthMax)
                    {
                        AdjustAddHeatlhOverflow(adjustment);
                    }
                    else if (HealthCurrent < HealthMax)
                    {
                        AdjustAddHealth(adjustment);
                    }
                }

                OnChangeHealth?.Invoke();
                if (adjustment > 0)
                    OnAddHealth?.Invoke();

                if (IsDead)
                {
                    OnDeath?.Invoke();
                }
            }
        }

        private void AdjustSubHealth(float adjustment)
        {
            var health = Health;
            var healthAddon = HealthAddon;

            var left = AdjustValue(ref healthAddon, adjustment, 0, HealthBonus);
            AdjustValue(ref health, left, 0, HealthMax);

            HealthAddon = healthAddon;
            Health = health;
        }

        private void AdjustAddHealth(float adjustment)
        {
            var health = Health;
            var left = AdjustValue(ref health, adjustment, 0, HealthMax);
            Health = health;
        }

        private void AdjustAddHeatlhOverflow(float adjustment)
        {
            var health = Health;
            var healthAddon = HealthAddon;

            AdjustValue(ref health, HealthMax, 0, HealthMax);
            AdjustValue(ref healthAddon, adjustment, 0, HealthBonus);

            Health = health;
            HealthAddon = healthAddon;
        }


        public void SetHealth(float value)
        {
            if (value < HealthMax)
            {
                Health = value;
                HealthAddon = 0;
            }
            else
            {
                Health = HealthMax;
                HealthAddon = Mathf.Clamp(value - HealthMax, 0, HealthAddonMax);
            }

            OnChangeHealth?.Invoke();

            if (IsDead)
            {
                OnDeath?.Invoke();
            }
        }

        public void RefillHealth()
        {
            Health = HealthMax;
            HealthAddon = 0;

            OnChangeHealth?.Invoke();
        }

        public void KillPlayer()
        {
            Health = 0;
            OnDeath?.Invoke();
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        private float AdjustValue(ref float current, float adjustment, float min, float max)
        {
            var result = current + adjustment;

            if (result < min)
            {
                current = min;
                return result - min;
            }

            if (result > max)
            {
                current = max;
                return result - max;
            }

            current += adjustment;
            return 0;
        }
    }
}
