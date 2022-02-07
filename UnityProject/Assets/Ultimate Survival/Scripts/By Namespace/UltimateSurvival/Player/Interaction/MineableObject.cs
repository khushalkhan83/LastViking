using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace UltimateSurvival
{
    public class MineableObject : MonoBehaviour
    {
        public Message Destroyed = new Message();

        #region Data
#pragma warning disable 0649

        [ObscuredID(typeof(FPTool.ToolPurpose))]
        [SerializeField] private ObscuredInt _toolPurpose;

        [SerializeField] private GameObject _root;
        [SerializeField] private Transform _dropLootPoint;
        [Header("On Destroy")]

        [SerializeField] protected GameObject m_DestroyedObject;

        [SerializeField] private ObscuredVector3 _offsetObjectDestroyed;

        [Header("Loot")]

        [SerializeField] private LootItem m_Loot;

        [SerializeField] private ObscuredFloat _amountResources;

        [Header("Bonus for mine metal tool")]
        [SerializeField] private string _itemName;
        [SerializeField] private int _bonusCount;

        [SerializeField] private List<string> Items;
        [SerializeField] private UnityEvent _onAmountDecreased;
#pragma warning restore 0649
        #endregion

        public GameObject Root => _root;

        public float Amount
        {
            get => _amountResources;
            protected set => _amountResources = value;
        }

        public Vector3 OffsetObjectDestroyed => _offsetObjectDestroyed;

        public FPTool.ToolPurpose RequiredToolPurpose => (FPTool.ToolPurpose)(int)_toolPurpose;
        protected InventoryModel InventoryModel => ModelsSystem.Instance._inventoryModel;
        protected HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;
        protected ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        protected ActionsLogModel ActionsLogModel => ModelsSystem.Instance._actionsLogModel;
        protected ViewsSystem ViewsSystem => ViewsSystem.Instance;
        protected RemoteSettingsModel RemoteSettingsModel => ModelsSystem.Instance._remoteSettingsModel;
        protected PurchasesModel PurchasesModel => ModelsSystem.Instance._purchasesModel;
        protected InventoryViewModel InventoryViewModel => ModelsSystem.Instance._inventoryViewModel;
        protected NetworkModel NetworkModel => ModelsSystem.Instance._networkModel;
        protected StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        protected MineableObjectsModel MineableObjectsModel =>  ModelsSystem.Instance._mineableObjectsModel;
        protected ResourcesExtractionModel ResourcesExtractionModel => ModelsSystem.Instance._resourcesExtractionModel;
        protected InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
        protected DropItemModel DropItemModel => ModelsSystem.Instance._dropItemModel;

        public InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }
        // public int LeftItemsCached { get; private set; }
        protected Vector3 DropLootPoint => _dropLootPoint != null ? _dropLootPoint.position : transform.position + Vector3.up;

        /// <summary>
        /// Gives the player the loot.
        /// </summary>
        public virtual void OnToolHit(Ray cameraRay, RaycastHit hitInfo, ExtractionSetting[] settings)
        {
            // Is the tool used good for destroying this type of object?
            var tool = settings.FirstOrDefault(x => x.ToolID == RequiredToolPurpose);

            // If so, do damage, and also give the player loot.
            if (tool != null)
            {
                var extraction = ResourcesExtractionModel.GetModifier(tool.ToolID) * tool.ExtractionRate;
                
                int oldAmount = Mathf.CeilToInt(Amount);
                DecreaseAllAmount(extraction);
                int newAmount = Mathf.CeilToInt(Amount);

                int addResourcesCount = oldAmount - newAmount;
                if(addResourcesCount > 0)
                {
                    DropLoot(addResourcesCount);
                }
            }
        }


        protected void DropLoot(int count, bool isLastExtraction = false)
        {
            var itemAdd = ItemsDB.GetItem(m_Loot.ItemName);
            SavableItem dropItem = new SavableItem(itemAdd, count);
            DropItemModel.DropItemFloating(dropItem, DropLootPoint, true, true);

            if(count > 0)
            {
                StatisticsModel.GatherResource((uint)count);

                var itemName = HotBarModel.EquipCell.Item.Name;
                bool isMetalTool = GetIsMetalTool(itemName);

                if(isLastExtraction && isMetalTool && _bonusCount > 0)
                {
                    var bonusItemData = ItemsDB.GetItem(_itemName);
                    SavableItem bonusItem = new SavableItem(bonusItemData, _bonusCount);
                    DropItemModel.DropItemFloating(bonusItem, DropLootPoint, true, true);
                }
            }
        }

        private bool GetIsMetalTool(string itemName)
        {
            bool answer = false;

            foreach (var item in Items)
            {
                if (item == itemName)
                {
                    answer = true;
                    return true;
                }
            }

            return answer;
        }

        private void OpenInventoryFillPopup()
        {
            InventoryIsFillPopupView = ViewsSystem.Show<InventoryIsFillPopupView>(ViewConfigID.InventoryIsFillPopup);
            InventoryIsFillPopupView.OnExpand += OnExpandInventoryIsFillPopupViewHandler;
            InventoryIsFillPopupView.OnBack += OnBackInventoryIsFillPopupViewHandler;
        }

        private void CloseInventoryIsFillPopup()
        {
            InventoryIsFillPopupView.OnBack -= OnBackInventoryIsFillPopupViewHandler;
            InventoryIsFillPopupView.OnExpand -= OnExpandInventoryIsFillPopupViewHandler;

            ViewsSystem.Hide(InventoryIsFillPopupView);
            InventoryIsFillPopupView = null;
        }

        private void OnBackInventoryIsFillPopupViewHandler()
        {
            CloseInventoryIsFillPopup();

            // DropItem(LeftItemsCached);
        }

        private void OnExpandInventoryIsFillPopupViewHandler()
        {
            CloseInventoryIsFillPopup();

            var watchPurchase = PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.InventoryExpandWatch);
            var remouteSettingsInventory = RemoteSettingsModel.Get<InventoryRemoteSettings>(RemoteSettingID.Inventory);
            var isShowExpandInventoryWatch = !remouteSettingsInventory.IsBuySlotsGold && (watchPurchase.IsCanPurchase || NetworkModel.IsHasConnection);

            if (isShowExpandInventoryWatch)
            {
                PurchasesModel.Purchase(PurchaseID.InventoryExpandWatch, OnPurchaseInventoryExpandWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.PlayerInventoryExpandGold, OnPurchaseInventoryExpandGold);
            }
        }

        private void OnPurchaseInventoryExpandWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PurchaseInventory();
            }
        }

        private void OnPurchaseInventoryExpandGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                PurchaseInventory();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private void PurchaseInventory()
        {
            InventoryModel.ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);

            InventoryViewModel.ExpandCellsOnce();
        }

        protected void DecreaseAllAmount(float amount)
        {
            Amount -= amount;

            _onAmountDecreased?.Invoke();

            if (Amount <= 0)
            {
                DestroyObject();
            }
        }

        protected virtual void DestroyObject()
        {
            if (m_DestroyedObject)
            {
                Instantiate(m_DestroyedObject, transform.position + transform.TransformVector(OffsetObjectDestroyed), Quaternion.identity);
            }

            Destroy(Root);

            if (Destroyed != null)
                Destroyed.Send();
        }
    }
}
