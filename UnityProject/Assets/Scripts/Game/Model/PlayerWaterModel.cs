using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.ComponentModel;
using UnityEngine;

namespace Game.Models
{
    public class PlayerWaterModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat Water;
            public ObscuredFloat WaterAddon;
            public ObscuredInt AddonLevel;

            public override SaveTime TimeSave { get => SaveTime.Deffered; }

            public void SetWater(float water)
            {
                Water = water;
                ChangeData();
            }

            public void SetWaterAddon(float waterAddon)
            {
                WaterAddon = waterAddon;
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

        [SerializeField] private ObscuredFloat _waterMax;
        [SerializeField] private ObscuredFloat[] _addonLevels;
        [SerializeField] private ObscuredFloat _waterBonus;
        [SerializeField] private ObscuredFloat _waterToThirst;

        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public ObscuredFloat Water
        {
            get => _data.Water;
            private set => _data.SetWater(value);
        }

        public ObscuredFloat WaterAddon
        {
            get => _data.WaterAddon;
            private set => _data.SetWaterAddon(value);
        }

        public ObscuredInt AddonLevel
        {
            get => _data.AddonLevel;
            protected set => _data.SetAddonLevel(value);
        }

        public float EquipmentBonus { get; private set;}

        public ObscuredFloat[] AddonLevels => _addonLevels;
        public float WaterCurrent => Water + WaterAddon;
        public float WaterCurrentMax => WaterCurrent > WaterMax ? WaterCurrent : WaterMax;
        public float WaterMax => _waterMax + EquipmentBonus;
        public float WaterAddonMax => GetAddonMax(AddonLevel);
        public float WaterBonus => _waterBonus;
        public float WaterToThirst => _waterToThirst;

        public event Action OnChangeWater;
        public event Action OnAddWater;
        public event Action OnChangeAddonLevel;
        public event Action OnChangeEquipmentBonus;

        public event Action OnStartThirsting;
        public event Action OnStopThirsting;

        public bool IsCantThirst { get; private set; }
        public bool IsThirsting { set; get; }

        public StorageModel StorageModel => _storageModel;

        public float GetAddonMax(int level)
        {
            if (level < AddonLevels.Length)
            {
                return AddonLevels[level];
            }

            return AddonLevels[AddonLevels.Length - 1];
        }

        public void StartThirst()
        {
            if (!IsThirsting)
            {
                IsThirsting = true;
                OnStartThirsting?.Invoke();
            }
        }

        public void StopThirst()
        {
            if (IsThirsting)
            {
                IsThirsting = false;
                OnStopThirsting?.Invoke();
            }
        }

        public void CantThirst(bool isCantThirst)
        {
            IsCantThirst = isCantThirst;
        }

        public void SetAddonLevel(int level)
        {
            AddonLevel = level;
            OnChangeAddonLevel?.Invoke();
        }

        public void AddAddon()
        {
            var water = Water;
            var left = AdjustValue(ref water, WaterAddonMax, 0, WaterMax);
            Water = water;

            if (left > 0)
            {
                var waterAddon = WaterAddon;
                AdjustValue(ref waterAddon, left, 0, WaterAddonMax);
                WaterAddon = waterAddon;
            }

            OnChangeWater?.Invoke();
            OnAddWater?.Invoke();
        }

        public void AddBonus()
        {
            var water = Water;
            var left = AdjustValue(ref water, WaterBonus, 0, WaterMax);
            Water = water;

            if (left > 0)
            {
                var waterAddon = WaterAddon;
                AdjustValue(ref waterAddon, left, 0, WaterBonus);
                WaterAddon = waterAddon;
            }

            OnChangeWater?.Invoke();
            OnAddWater?.Invoke();
        }

        public void SetEquipmentBonus(float bonus)
        {
            EquipmentBonus = bonus;

            if(Water > WaterMax) // Set water to max value after equipment bonuse reduced and water biger than max water
            {
                Water = WaterMax;
                OnChangeWater?.Invoke();
            }
            else if(Water < WaterMax && WaterAddon > 0) // Move water points from WaterAddon to Water when equipment bonuse increased
            {
                float needWaterToMax = WaterMax - Water;

                var water = Water;
                var waterAddon = WaterAddon;

                var left = AdjustValue(ref waterAddon, -needWaterToMax, 0, WaterBonus);

                float addToWater = needWaterToMax + left;
                AdjustValue(ref water, addToWater, 0, WaterMax);

                WaterAddon = waterAddon;
                Water = water;

                OnChangeWater?.Invoke();
            }

            OnChangeEquipmentBonus?.Invoke();
        }

        public void AdjustWater(float adjustment)
        {
            if (!IsCantThirst)
            {
                if (adjustment < 0)
                {
                    AdjustSubWater(adjustment);
                }
                else
                {
                    AdjustAddWater(adjustment);
                }

                OnChangeWater?.Invoke();
                if (adjustment > 0)
                    OnAddWater?.Invoke();
            }
        }

        public void AdjustWater(float adjustment, bool isOverflow)
        {
            if (!IsCantThirst)
            {
                if (adjustment < 0)
                {
                    AdjustSubWater(adjustment);
                }
                else
                {
                    if (isOverflow && Water + adjustment > WaterMax)
                    {
                        AdjustAddWaterOverflow(adjustment);
                    }
                    else if (WaterCurrent < WaterMax)
                    {
                        AdjustAddWater(adjustment);
                    }
                }

                OnChangeWater?.Invoke();
                if (adjustment > 0)
                    OnAddWater?.Invoke();
            }
        }

        private void AdjustSubWater(float adjustment)
        {
            var water = Water;
            var waterAddon = WaterAddon;

            var left = AdjustValue(ref waterAddon, adjustment, 0, WaterBonus);
            AdjustValue(ref water, left, 0, WaterMax);

            WaterAddon = waterAddon;
            Water = water;
        }

        private void AdjustAddWater(float adjustment)
        {
            var water = Water;
            var left = AdjustValue(ref water, adjustment, 0, WaterMax);
            Water = water;
        }

        private void AdjustAddWaterOverflow(float adjustment)
        {
            var water = Water;
            var waterAddon = WaterAddon;

            AdjustValue(ref water, WaterMax, 0, WaterMax);
            AdjustValue(ref waterAddon, adjustment, 0, WaterBonus);

            Water = water;
            WaterAddon = waterAddon;
        }

        public void SetWater(float value)
        {
            Water = Mathf.Clamp(value, 0, WaterMax);

            OnChangeWater?.Invoke();
        }

        public void RefillWater()
        {
            Water = WaterMax;
            WaterAddon = 0;

            OnChangeWater?.Invoke();
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        private float AdjustValue(ref ObscuredFloat current, ObscuredFloat adjustment, ObscuredFloat min, ObscuredFloat max)
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
