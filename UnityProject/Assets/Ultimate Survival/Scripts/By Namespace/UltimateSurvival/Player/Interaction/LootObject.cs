using cakeslice;
using Core.Storage;
using Core.Views;
using Game.Controllers;
using Game.Models;
using Game.ObjectPooling;
using Game.Views;
using RoboRyanTron.SearchableEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public class LootObject : InteractableObject, IData,IOutlineTarget, IResettable
    {
        [Serializable]
        public class Data : DataBase
        {
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
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private bool isSavable;
        [SerializeField] private bool _isStorage;
        [SerializeField] private int _countOpenStart;
        [SerializeField] private ExpandGroup _expandGroup;
        [SearchableEnum]
        [SerializeField] private LocalizationKeyID _nameKeyID;
        [SerializeField] private CellSpawnSettings[] _cellsSettings;
        [SerializeField] List<Renderer> _renderers;

#pragma warning restore 0649
        #endregion

        public bool IsSavable => isSavable;
        public bool IsStorage => _isStorage;

        public int CountOpenStart => _countOpenStart;
        public ExpandGroup ExpandGroup => _expandGroup;
        public LocalizationKeyID NameKeyID => _nameKeyID;
        public ItemsContainer ItemsContainer => _data.ItemsContainer;
        public CellSpawnSettings[] CellsSettings => _cellsSettings;

        //

        public ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;
        public StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        //

        public bool IsOpened { get; private set; }
        public bool IsEmpty { get; private set; }

        public bool IsCanOpen => isActiveAndEnabled;

        //

        public event Action OnOpen;
        public event Action OnClose;
        public event Action OnDataInitialize;
        public event Action<IOutlineTarget> OnUpdateRendererList;

        public InventoryLootView InventoryLootView { get; private set; }

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        public void Open()
        {
            if (!IsOpened)
            {

                if (!IsSavable)
                {
                    GenerateItems();
                }
                else if (!StorageModel.TryProcessing(_data))
                {
                    GenerateItems();
                }
                ItemsContainer.OnLoad();
            }

            IsOpened = true;

            if (enabled)
            {
                InventoryLootViewControllerData data = new InventoryLootViewControllerData(this);
                InventoryLootView = ViewsSystem.Show<InventoryLootView>(ViewConfigID.InventoryLoot, data);
                InventoryLootView.OnHide += OnHideHandler;
                OnOpen?.Invoke();
            }
        }

        public void LoadData()
        {
            if(!IsOpened)
            {
                StorageModel.TryProcessing(_data);
                ItemsContainer.OnLoad();
            }
        }

        private void GenerateItems()
        {
            foreach (var settings in CellsSettings)
            {
                var item = settings.GenerateItem(ItemsDB.ItemDatabase, ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item.ItemData));
                ItemsContainer.AddCell(item);
            }
            OnUpdateRendererList?.Invoke(this);
        }

        private void OnHideHandler(IView view)
        {
            InventoryLootView.OnHide -= OnHideHandler;
            Close();
        }

        private void Close()
        {
            IsEmpty = ItemsContainer.Cells.All(x => x.IsEmpty);
            if (IsEmpty)
            {
                foreach(Renderer r in _renderers)
                {
                    if (r!=null)
                    {
                        Outline o = r.GetComponent<Outline>();
                        if (o != null)
                            Destroy(o);
                    }
                }
                OnUpdateRendererList?.Invoke(this);
            }
            OnClose?.Invoke();
        }

        private void OnEnable()
        {
            _data.Initialize();
        }

        public void UUIDInitialize() => OnDataInitialize?.Invoke();

        public List<Renderer> GetRenderers()
        {
            if (IsOpened && IsEmpty )
            {
                return new List<Renderer>();
            }
            else
            {
                return _renderers;
            }
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }

        public int GetColor()
        {
            return 1;
        }

        public void ResetObject()
        {
            IsOpened = false;
            IsEmpty = false;
            ItemsContainer.RemoveLastCells(ItemsContainer.CountCells);
        }
    }
}
