using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class FurnaceModel : MonoBehaviour, IData
    {
        [Serializable]
        private class Data : DataBase
        {
            #region Data
#pragma warning disable 0649

            public float FuelAmount;
            public float BoostRemainingTime;
            public bool IsFire;
            public ItemsContainer ItemsContainer;
            public float FuelMultiplier;


            public void Initialize()
            {
                ItemsContainer.OnAddCell += OnAddCell;
                ItemsContainer.OnAddItems += OnAddItems;
                ItemsContainer.OnChangeCell += OnChangeCell;
            }

            private void OnChangeCell(CellModel cell) => ChangeData();
            private void OnAddItems(int itemId, int count) => ChangeData();
            private void OnAddCell(CellModel cell) => ChangeData();

            public void SetFuelAmount(float amount)
            {
                FuelAmount = amount;
                ChangeData();
            }

            public void SetBoostRemainingTime(float time)
            {
                BoostRemainingTime = time;
                ChangeData();
            }

            public void SetIsFire(bool fire)
            {
                IsFire = fire;
                ChangeData();
            }

            public void SetFuelMultiplier(float multiplier) {
                FuelMultiplier = multiplier;
                ChangeData();
            }

#pragma warning restore 0649
            #endregion
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private float _fuelPerSeconds;
        [SerializeField] private float _boostTime;
        [SerializeField] private float _boostMultiplier;
        [SerializeField] private ExpandGroup _expandGroup;
        [SerializeField] private int _countCellsStart;

#pragma warning restore 0649
        #endregion

        public float BoostTime => _boostTime;
        public float BoostMultiplier => _boostMultiplier;
        public float FuelPerSeconds => _fuelPerSeconds;
        public ExpandGroup ExpandGroup => _expandGroup;
        public int CountCellsStart => _countCellsStart;

        public ItemsContainer ItemsContainer => _data.ItemsContainer;

        public float FuelAmount
        {
            get
            {
                return _data.FuelAmount;
            }
            protected set
            {
                _data.SetFuelAmount(value);
            }
        }

        public float BoostRemainingTime
        {
            get
            {
                return _data.BoostRemainingTime;
            }
            protected set
            {
                _data.SetBoostRemainingTime(value);
            }
        }

        public bool IsFire
        {
            get
            {
                return _data.IsFire;
            }
            set
            {
                _data.SetIsFire(value);
            }
        }

        public float FuelMultiplier
        {
            get => _data.FuelMultiplier;
            set => _data.SetFuelMultiplier(value);
        }

        public bool IsBoost => BoostRemainingTime > 0;
        public int MeltableItemsCount => ItemsContainer.Cells.Count(c => c.IsHasItem && c.Item.HasProperty("Melting time"));

        //

        public event Action OnChangeFireState;
        public event Action OnStartBoost;
        public event Action OnUpdateBoostRemainingTime;
        public event Action OnEndBoost;
        public event Action<SavableItem> OnDrop;
        public event Action OnDataInitialize;

        //

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        public uint UUID { get; private set; }

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        private void Start()
        {
            if (!StorageModel.TryProcessing(_data))
            {
                ItemsContainer.AddCells(CountCellsStart);

                var campfire = ItemsDB.GetItem("items_furnace");
                foreach (var item in campfire.Recipe.RequiredItems)
                {
                    if (item.Name == "Wood")
                    {
                        ItemsContainer.AddItems(ItemsDB.GetItem(item.Name).Id, item.Amount);
                    }
                }
            }
            _data.Initialize();
            ItemsContainer.OnLoad();
        }

        public void Boost()
        {
            if (IsBoost)
            {
                OnEndBoost?.Invoke();
            }

            BoostRemainingTime = BoostTime;

            OnStartBoost?.Invoke();
        }

        public void SetFire(bool isFire)
        {
            var last = IsFire;
            IsFire = isFire;

            if (IsFire != last)
            {
                OnChangeFireState?.Invoke();
            }
        }

        public void FireProcessing(float deltaTime)
        {
            foreach (var cell in ItemsContainer.Cells)
            {
                if (cell.IsHasItem)
                {
                    CookingProcess(cell, deltaTime);
                }
            }

            FuelAmount -= FuelPerSeconds * deltaTime;
            if (FuelAmount <= 0)
            {
                if (!TryAddFuel())
                {
                    SetFire(false);
                }
            }
        }

        public void BoostProcessing(float deltaTime)
        {
            BoostRemainingTime -= deltaTime;

            OnUpdateBoostRemainingTime?.Invoke();

            if (BoostRemainingTime <= 0)
            {
                BoostRemainingTime = 0;
                OnEndBoost?.Invoke();
            }
        }

        private void CookingProcess(CellModel cell, float deltaTime)
        {
            if (cell.Item.TryGetProperty("Melting time", out var cookingTime))
            {
                cookingTime.Float.Current -= GetDeltaTimeCook(deltaTime);

                if (cookingTime.Float.Current <= 0)
                {
                    cookingTime.Float.Current = cookingTime.Float.Default;
                    cell.Item.AdjustCount(-1);

                    if (cell.Item.TryGetProperty("Melting result", out var cookingResult))
                    {
                        var itemData = ItemsDB.GetItem(cookingResult.String);
                        var left = 0;
                        if (cell.Item.TryGetProperty("Melting result count", out var cookingResultCount))
                        {
                            left = ItemsContainer.AddItems(itemData.Id, cookingResultCount.Int.Current);
                        }
                        else
                        {
                            left = ItemsContainer.AddItems(itemData.Id, 1);
                        }

                        if (left > 0)
                        {
                            var dropItem = new SavableItem(itemData, left);
                            OnDrop?.Invoke(dropItem);
                        }
                    }
                }

                cell.Item.SetProperty(cookingTime);
            }
        }

        private float GetDeltaTimeCook(float deltaTime)
        {
            if (FuelMultiplier != 0)
            {
                deltaTime *= FuelMultiplier;
            }
        
            if (IsBoost)
            {
                return deltaTime * BoostMultiplier;
            }

            return deltaTime;
        }

        private bool TryAddFuel()
        {
            var fuel = ItemsContainer.Cells.FirstOrDefault(x => x.IsHasItem && x.Item.HasProperty("IsFuel"));
            if (fuel != null)
            {
                fuel.Item.AdjustCount(-1);
                FuelAmount += fuel.Item.GetProperty("IsFuel").Float.Default;

                if (fuel.Item.HasProperty("FuelMultiplier"))
                {
                    FuelMultiplier = fuel.Item.GetProperty("FuelMultiplier").Float.Default;
                }
                else 
                {
                    FuelMultiplier = 1;
                }

                return true;
            }

            return false;
        }

        public void UUIDInitialize() => OnDataInitialize?.Invoke();
    }
}
