using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class CampFireModel : MonoBehaviour, IData
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

            public void SetFuelMultiplier(float multiplier)
            {
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

        public int CountCellsStart => _countCellsStart;
        public float BoostTime => _boostTime;
        public float BoostMultiplier => _boostMultiplier;
        public float FuelPerSeconds => _fuelPerSeconds;
        public ExpandGroup ExpandGroup => _expandGroup;
        public ItemsContainer ItemsContainer => _data.ItemsContainer;

        public float FuelAmount
        {
            get => _data.FuelAmount;
            protected set => _data.SetFuelAmount(value);
        }

        public float BoostRemainingTime
        {
            get => _data.BoostRemainingTime;
            protected set => _data.SetBoostRemainingTime(value);
        }

        public bool IsFire
        {
            get => _data.IsFire;
            set => _data.SetIsFire(value);
        }

        public float FuelMultiplier
        {
            get => _data.FuelMultiplier;
            set => _data.SetFuelMultiplier(value);
        }

        public bool IsBoost => BoostRemainingTime > 0;
        public float BoostRemainingTimeNormalized => BoostRemainingTime / BoostTime;
        public int CookableItemsCount => ItemsContainer.Cells.Count(c => c.IsHasItem && c.Item.HasProperty("Cooking time"));
        public int CookableBoostableItemsCount => ItemsContainer.Cells.Count(c => c.IsHasItem && c.Item.HasProperty("Cooking time") && CanBeBoosted(c.Item));
        public bool HasCookableBoostableItems => CookableBoostableItemsCount > 0;

        //

        public event Action OnChangeFireState;
        public event Action OnStartBoost;
        public event Action OnUpdateBoostRemainingTime;
        public event Action<SavableItem> OnDrop;
        public event CookItem OnCook;
        public event Action OnEndBoost;
        public event Action OnDataInitialize;

        //
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

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

                var campfire = ItemsDB.GetItem("items_campfire");
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
            BoostRemainingTime = BoostTime;

            OnStartBoost?.Invoke();
        }

        public void SetFire(bool isFire)
        {
            IsFire = isFire;
            OnChangeFireState?.Invoke();
        }

        public bool BlockCoockingOnFire {get; set;}

        public void OnUpdate(float deltaTime)
        {
            if(BlockCoockingOnFire) return;

            if (IsFire)
            {
                FuelAmount -= FuelPerSeconds * deltaTime;
                if (FuelAmount <= 0)
                {
                    if (!TryAddFuel())
                    {
                        SetFire(false);
                    }
                }
            }

            if (IsFire)
            {
                foreach (var cell in ItemsContainer.Cells)
                {
                    if (cell.IsHasItem)
                    {
                        CookingProcess(cell, deltaTime);
                    }
                }
            }

            if (BoostRemainingTime > 0)
            {
                BoostRemainingTime -= Time.deltaTime;

                OnUpdateBoostRemainingTime?.Invoke();

                if (BoostRemainingTime <= 0)
                {
                    BoostRemainingTime = 0;
                    OnEndBoost?.Invoke();
                }
            }
        }

        private void CookingProcess(CellModel cell, float deltaTime)
        {
            if (cell.Item.TryGetProperty("Cooking time", out var cookingTime))
            {
                ItemProperty.Value cookingResult = default;
                var cookingTimeValue = cookingTime.Float;

                bool canBeBoosted = CanBeBoosted(cell.Item);
                cookingTimeValue.Current -= GetDeltaTimeCook(deltaTime, canBeBoosted);

                var timeCurrent = cookingTimeValue.Current;

                if (timeCurrent <= 0)
                {
                    cookingTimeValue.Current = cookingTimeValue.Default - timeCurrent;
                    cell.Item.AdjustCount(-1);

                    if (cell.Item.TryGetProperty("Cooking result", out cookingResult))
                    {
                        var itemData = ItemsDB.GetItem(cookingResult.String);
                        int left;
                        if (cell.Item.TryGetProperty("Cooking result count", out var cookingResultCount))
                        {
                            left = ItemsContainer.AddItems(itemData.Id, cookingResultCount.Int.Current);
                            OnCook?.Invoke(itemData.Name, cookingResultCount.Int.Current);
                        }
                        else
                        {
                            left = ItemsContainer.AddItems(itemData.Id, 1);
                            OnCook?.Invoke(itemData.Name, 1);
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

        public bool CanBeBoosted(SavableItem item) => !item.HasProperty("CanBeOvercooked");

        private float GetDeltaTimeCook(float deltaTime, bool canBeBoosted)
        {
            if (FuelMultiplier != 0)
            {
                deltaTime *= FuelMultiplier;
            }

            if (IsBoost && canBeBoosted)
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
