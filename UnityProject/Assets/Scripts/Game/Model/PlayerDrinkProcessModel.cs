using Core.Storage;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class PlayerDrinkProcessModel : MonoBehaviour
    {
        [Serializable]
        public class DrinkData
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private float _allTime;
            [SerializeField] private float _remainingTime;
            [SerializeField] private float _drinkValue;
            [SerializeField] private bool _isOverflow;

#pragma warning restore 0649
            #endregion

            public float AllTime
            {
                get => _allTime;
                set => _allTime = value;
            }

            public float RemainingTime
            {
                get => _remainingTime;
                set => _remainingTime = value;
            }

            public float DrinkValue
            {
                get => _drinkValue;
                set => _drinkValue = value;
            }

            public bool IsOverflow
            {
                get => _isOverflow;
                set => _isOverflow = value;
            }
        }

        [Serializable]
        public class Data : DataBase
        {
            public List<DrinkData> DrinkDatas;

            public void DrinkProcess() => ChangeData();

            public void AddDrink(DrinkData data)
            {
                DrinkDatas.Add(data);
                ChangeData();
            }

            public void RemoveDrinkAt(int index)
            {
                DrinkDatas.RemoveAt(index);
                ChangeData();
            }

            public void ClearDrinks()
            {
                DrinkDatas.Clear();
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public Data _Data => _data;

        public List<DrinkData> DrinkDatas => _data.DrinkDatas;

        public bool IsHasDrink => _data.DrinkDatas.Count > 0;

        public StorageModel StorageModel => _storageModel;

        public event Action OnAddDrink;
        public event Action OnLoadData;
        public event Action OnEndDrink;

        public bool IsInitializeData { get; private set; }

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

        public void EndDrink()
        {
            OnEndDrink?.Invoke();
        }

        public void UpdateDrinkValues(float deltaTime)
        {
            DrinkData drinkData;

            for (int i = DrinkDatas.Count - 1; i >= 0; i--)
            {
                drinkData = DrinkDatas[i];
                drinkData.RemainingTime -= deltaTime;

                if (drinkData.RemainingTime <= 0)
                {
                    _Data.RemoveDrinkAt(i);
                }
            }

            if (DrinkDatas.Count > 0)
                _Data.DrinkProcess();
        }

        public bool GetIsOverflow(DrinkData data) => data.IsOverflow;

        public float GetDrinkValue(DrinkData drink, float deltaTime)
        {
            var drinkPerSecond = drink.DrinkValue / drink.AllTime;
            var time = Mathf.Min(drink.RemainingTime, deltaTime);

            return drinkPerSecond * time;
        }

        public void AddDrink(float time, float drink, bool isOverflow)
        {
            var drinkData = new DrinkData()
            {
                AllTime = time,
                RemainingTime = time,
                DrinkValue = drink,
                IsOverflow = isOverflow
            };

            _Data.AddDrink(drinkData);

            OnAddDrink?.Invoke();
        }
    }
}
