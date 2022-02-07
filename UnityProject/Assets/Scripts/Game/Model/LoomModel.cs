using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class LoomModel : MonoBehaviour, IData
    {
        [Serializable]
        private class Data : DataBase
        {
            #region Data
#pragma warning disable 0649

            public float BoostRemainingTime;
            public bool IsWeave;
            public ItemsContainer ItemsContainer;


            public void Initialize()
            {
                ItemsContainer.OnAddCell += OnAddCell;
                ItemsContainer.OnAddItems += OnAddItems;
                ItemsContainer.OnChangeCell += OnChangeCell;
            }

            private void OnChangeCell(CellModel cell) => ChangeData();
            private void OnAddItems(int itemId, int count) => ChangeData();
            private void OnAddCell(CellModel cell) => ChangeData();

            public void SetBoostRemainingTime(float time)
            {
                BoostRemainingTime = time;
                ChangeData();
            }

            public void SetIsWeave(bool isWeave)
            {
                IsWeave = isWeave;
                ChangeData();
            }

#pragma warning restore 0649
            #endregion
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private float _boostTime;
        [SerializeField] private float _boostMultiplier;
        [SerializeField] private ExpandGroup _expandGroup;
        [SerializeField] private int _countCellsStart;

#pragma warning restore 0649
        #endregion

        public float BoostTime => _boostTime;
        public float BoostMultiplier => _boostMultiplier;
        public ExpandGroup ExpandGroup => _expandGroup;
        public int CountCellsStart => _countCellsStart;

        public ItemsContainer ItemsContainer => _data.ItemsContainer;

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

        public bool IsWeave
        {
            get
            {
                return _data.IsWeave;
            }
            protected set
            {
                _data.SetIsWeave(value);
            }
        }

        public bool IsBoost => BoostRemainingTime > 0;
        public int WeavableItemsCount => ItemsContainer.Cells.Count(c => c.IsHasItem && c.Item.HasProperty("Weaving time"));

        //

        public event Action OnChangeWeaveState;
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

                var loom = ItemsDB.GetItem("items_loom");
                foreach (var item in loom.Recipe.RequiredItems)
                {
                    if (item.Name == "resource_sugarcane")
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

        public void SetWeave(bool isWeave)
        {
            var last = IsWeave;
            IsWeave = isWeave;

            if (IsWeave != last)
            {
                OnChangeWeaveState?.Invoke();
            }
        }

        public void WeaweProcessing(float deltaTime)
        {
            foreach (var cell in ItemsContainer.Cells)
            {
                if (cell.IsHasItem)
                {
                    WeavingProcess(cell, deltaTime);
                }
            }
        }

        public void BoostProcessing(float deltaTime)
        {
            BoostRemainingTime -= Time.deltaTime;

            OnUpdateBoostRemainingTime?.Invoke();

            if (BoostRemainingTime <= 0)
            {
                BoostRemainingTime = 0;
                OnEndBoost?.Invoke();
            }
        }

        private void WeavingProcess(CellModel cell, float deltaTime)
        {
            if (cell.Item.TryGetProperty("Weaving time", out var weavingTime))
            {
                weavingTime.Float.Current -= GetDeltaTimeCook(deltaTime);

                if (weavingTime.Float.Current <= 0)
                {
                    weavingTime.Float.Current = weavingTime.Float.Default;
                    cell.Item.AdjustCount(-1);

                    if (cell.Item.TryGetProperty("Weaving result", out var weavingResult))
                    {
                        var itemData = ItemsDB.GetItem(weavingResult.String);
                        var left = 0;
                        if (cell.Item.TryGetProperty("Weaving result count", out var cookingResultCount))
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

                cell.Item.SetProperty(weavingTime);
            }
        }

        private float GetDeltaTimeCook(float deltaTime)
        {
            if (IsBoost)
            {
                return deltaTime * BoostMultiplier;
            }

            return deltaTime;
        }

        public void UUIDInitialize() => OnDataInitialize?.Invoke();
    }
}
