using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerFoodModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredFloat Food;
            public ObscuredFloat FoodAddon;
            public ObscuredInt AddonLevel;

            public void SetFood(float food)
            {
                Food = food;
                ChangeData();
            }

            public void SetFoodAddon(float foodAddon)
            {
                FoodAddon = foodAddon;
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
        [SerializeField] private ObscuredFloat _foodMax;
        [SerializeField] private ObscuredFloat[] _addonLevels;
        [SerializeField] private ObscuredFloat _foodBonus;

        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public float Food
        {
            get
            {
                return _data.Food;
            }
            private set
            {
                _data.SetFood(value);
            }
        }

        public float FoodAddon
        {
            get
            {
                return _data.FoodAddon;
            }
            private set
            {
                _data.SetFoodAddon(value);
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
        public float FoodCurrent => Food + FoodAddon;
        public float FoodCurrentMax => FoodCurrent > FoodMax ? FoodCurrent : FoodMax;
        public float FoodMax => _foodMax + EquipmentBonus;
        public float FoodAddonMax => GetAddonMax(AddonLevel);
        public float FoodBonus => _foodBonus;

        public event Action OnChangeFood;
        public event Action OnAddFood;
        public event Action OnChangeAddonLevel;
        public event Action OnChangeEquipmentBonus;

        public event Action OnStartHungering;
        public event Action OnStopHungering;

        public bool IsCantHunger { get; private set; }
        public bool IsHungering { set; get; } = false;

        public StorageModel StorageModel => _storageModel;

        public float GetAddonMax(int level)
        {
            if (level < AddonLevels.Length)
            {
                return AddonLevels[level];
            }

            return AddonLevels[AddonLevels.Length - 1];
        }

        public void StartHunger()
        {
            if (!IsHungering)
            {
                IsHungering = true;
                OnStartHungering?.Invoke();
            }
        }

        public void StopHunger()
        {
            if (IsHungering)
            {
                IsHungering = false;
                OnStopHungering?.Invoke();
            }
        }

        public void CantHunger(bool isCantHunger)
        {
            IsCantHunger = isCantHunger;
        }

        public void SetAddonLevel(int level)
        {
            AddonLevel = level;
            OnChangeAddonLevel?.Invoke();
        }

        public void AddAddon()
        {
             var food = Food;
             var left = AdjustValue(ref food, FoodAddonMax, 0, FoodMax);
             Food = food;

             if (left > 0)
             {
                 var foodAddon = FoodAddon;
                 AdjustValue(ref foodAddon, left, 0, FoodAddonMax);
                 FoodAddon = foodAddon;
             }

             OnChangeFood?.Invoke();
             OnAddFood?.Invoke();
        }

        public void AddBonus()
        {
             var food = Food;
             var left = AdjustValue(ref food, FoodBonus, 0, FoodMax);
             Food = food;

             if (left > 0)
             {
                 var foodAddon = FoodAddon;
                 AdjustValue(ref foodAddon, left, 0, FoodBonus);
                 FoodAddon = foodAddon;
             }

             OnChangeFood?.Invoke();
             OnAddFood?.Invoke();
        }

        public void SetEquipmentBonus(float bonus)
        {
            EquipmentBonus = bonus;

            if(Food > FoodMax) // Set food to max value after equipment bonuse reduced and food biger than max food
            {
                Food = FoodMax;
                OnChangeFood?.Invoke();
            }
            else if(Food < FoodMax && FoodAddon > 0) // Move food points from FoodAddon to Food when equipment bonuse increased
            {
                float needFoodToMax = FoodMax - Food;

                var food = Food;
                var foodAddon = FoodAddon;

                var left = AdjustValue(ref foodAddon, -needFoodToMax, 0, FoodBonus);

                float addToFood = needFoodToMax + left;
                AdjustValue(ref food, addToFood, 0, FoodMax);

                FoodAddon = foodAddon;
                Food = food;

                OnChangeFood?.Invoke();
            }

            OnChangeEquipmentBonus?.Invoke();
        }

        public void AdjustFood(float adjustment)
        {
            if (!IsCantHunger)
            {
                if (adjustment < 0)
                {
                    AdjustSubFood(adjustment);
                }
                else
                {
                    AdjustAddFood(adjustment);
                }

                OnChangeFood?.Invoke();
                if (adjustment > 0)
                    OnAddFood?.Invoke();
            }
        }

        public void AdjustFood(float adjustment, bool isOverflow)
        {
            if (!IsCantHunger)
            {
                if (adjustment < 0)
                {
                    AdjustSubFood(adjustment);
                }
                else
                {
                    if (isOverflow && Food + adjustment > FoodMax)
                    {
                        AdjustAddFoodOverflow(adjustment);
                    }
                    else if (FoodCurrent < FoodMax)
                    {
                        AdjustAddFood(adjustment);
                    }
                }

                OnChangeFood?.Invoke();
                if (adjustment > 0)
                    OnAddFood?.Invoke();
            }
        }

        private void AdjustSubFood(float adjustment)
        {
            var food = Food;
            var foodAddon = FoodAddon;

            var left = AdjustValue(ref foodAddon, adjustment, 0, FoodBonus);
            AdjustValue(ref food, left, 0, FoodMax);

            FoodAddon = foodAddon;
            Food = food;
        }

        private void AdjustAddFood(float adjustment)
        {
            var food = Food;
            var left = AdjustValue(ref food, adjustment, 0, FoodMax);
            Food = food;
        }

        private void AdjustAddFoodOverflow(float adjustment)
        {
            var food = Food;
            var foodAddon = FoodAddon;

            AdjustValue(ref food, FoodMax, 0, FoodMax);
            AdjustValue(ref foodAddon, adjustment, 0, FoodBonus);

            Food = food;
            FoodAddon = foodAddon;
        }

        public void RefillFood()
        {
            Food = FoodMax;
            FoodAddon = 0;

            OnChangeFood?.Invoke();
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
