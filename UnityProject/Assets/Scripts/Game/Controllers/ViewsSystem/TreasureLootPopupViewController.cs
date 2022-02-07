using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TreasureLootPopupViewController : ViewControllerBase<TreasureLootPopupView>
    {
        [Inject] public AudioSystem AudioSystem { get; set; }
        [Inject] public ViewsSystem ViewsSystem { get; set; }
        [Inject] public LocalizationModel LocalizationModel { get; set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; set; }
        [Inject] public GameTimeModel GameTimeModel { get; set; }
        [Inject] public PurchasesModel PurchasesModel { get; set; }
        [Inject] public CoinsModel CoinsModel { get; set; }
        [Inject] public NetworkModel NetworkModel { get; set; }
        [Inject] public TreasureLootModel TreasureLootModel { get; set; }
        [Inject] public DropContainerModel DropContainerModel { get; set; }

        protected TreasureLootObject TreasureLootObject { get; set; }
        public virtual float RespawnSec => TreasureLootObject.RespawnSec;
        public virtual TreasureID TreasureID => TreasureLootObject.TreasureID;
        public virtual TreasureChestConfig TreasureChestConfig { get; set; }
        public virtual bool OpenLootBoxAfterDrop => true;
        public virtual PurchaseID X2WatchPurchase {
            get {
                if (TreasureChestConfig != null)
                    return TreasureChestConfig.X2Watch;
                else
                    return PurchaseID.None;
            }
        }
        public virtual PurchaseID X2GoldPurchase
        {
            get
            {
                if (TreasureChestConfig != null)
                    return TreasureChestConfig.X2Gold;
                else
                    return PurchaseID.None;
            }
        }
        public virtual PurchaseID RespinPurchase
        {
            get
            {
                if (TreasureChestConfig != null)
                    return TreasureChestConfig.Respin;
                else
                    return PurchaseID.None;
            }
        }
        protected virtual List<SavableItem> LootItems
        {
            get { return TreasureLootObject.LootItems; }
            set { TreasureLootObject.LootItems = value; }
        }
        protected virtual bool DoubleUsed
        {
            get { return TreasureLootObject.DoubleUsed; }
            set { TreasureLootObject.DoubleUsed = value; }
        }
        protected virtual bool Spined
        {
            get { return TreasureLootObject.Spined; }
            set { TreasureLootObject.Spined = value; }
        }
        protected virtual long TimeSpawnTicks
        {
            get { return TreasureLootObject.TimeSpawnTicks; }
            set { TreasureLootObject.TimeSpawnTicks = value; }
        }

        protected virtual void AddLootItem(SavableItem item) => TreasureLootObject.AddLootItem(item);

        protected virtual void RemoveLootItem(SavableItem item) => TreasureLootObject.RemoveLootItem(item);

        protected virtual void ReplaceLootItem(int index, SavableItem newItem) => TreasureLootObject.ReplaceLootItem(index, newItem);

        protected virtual void ClearLootItems() => TreasureLootObject.ClearLootItems();

        protected virtual void GenerateItems() => TreasureLootObject.GenerateItems();

        protected override void Show()
        {
            View.OnClose += OnCloseHandler;
            View.OnTakeVideo += OnDoubleVideo;
            View.OnTakeGold += OnDoubleGold;
            View.OnTakeAll += OnTakeAllHandler;
            View.OnRespin += OnRerollHandler;

            View.SetIsVisibleCloseButton(false);

            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            NetworkModel.OnInternetConnectionStateChange += OnInternetConnectionStateChangeHandler;
            NetworkModel.UpdateInternetConnectionStatus();

            Initialize();
            SetLocalization();
            OnChestOpen();
        }

        protected override void Hide()
        {
            View.OnClose -= OnCloseHandler;
            View.OnTakeVideo -= OnDoubleVideo;
            View.OnTakeGold -= OnDoubleGold;
            View.OnTakeAll -= OnTakeAllHandler;
            View.OnRespin -= OnRerollHandler;
            NetworkModel.OnInternetConnectionStateChange -= OnInternetConnectionStateChangeHandler;

            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
        }


        protected void OnInternetConnectionStateChangeHandler()
        {
            if (NetworkModel.IsHasConnection)
            {
                PurchasesModel.GetInfo<IWatchPurchase>(X2WatchPurchase).Prepere();
            }
        }

        protected void Initialize()
        {
            TreasureLootObject = PlayerEventHandler.RaycastData.Value?.GameObject?.GetComponent<TreasureLootObject>();
            TreasureChestConfig = TreasureLootModel.TreasureConfigProvider[TreasureID];
            InitializeLootItmes();
            SetupDoubleButtons();
            SetupSlotsViews();
            View.SetDoubleGoldPrice(PurchasesModel.GetInfo<IPurchaseCoinInfo>(X2GoldPurchase).CoinCost);
            if (!Spined)
            {
                SpinAllItems();
            }
        }

        protected virtual void OnChestOpen() { }

        protected virtual void InitializeLootItmes()
        {
            if (LootItems == null || LootItems.Count == 0)
            {
                GenerateItems();
            }
        }

        protected void SetupDoubleButtons()
        {
            if (DoubleUsed)
            {
                View.SetIsVisibleVideoButton(false);
                View.SetIsVisibleGoldButton(false);
            }
            else if (PurchasesModel.IsCanPurchase(X2WatchPurchase))
            {
                View.SetIsVisibleVideoButton(true);
                View.SetIsVisibleGoldButton(false);
            }
            else
            {
                View.SetIsVisibleVideoButton(false);
                View.SetIsVisibleGoldButton(true);
            }
        }

        protected void SetupSlotsViews()
        {
            SavableItem item;
            int i = 0;
            for (i = 0; i < LootItems.Count; i++)
            {
                item = LootItems[i];
                RouletteSlotView slot;
                if (i == View.Slots.Count)
                {
                    slot = View.AddSlot();
                }
                else
                {
                    slot = View.Slots[i];
                }

                slot.IsSpecial = TreasureLootModel.IsCellSpecial(TreasureID, i);
                slot.SetItem(item);
                slot.SetIsVisible(true);
            }

            for (; i < View.Slots.Count; i++)
            {
                View.Slots[i].SetIsVisible(false);
            }
            View.SetRespinGoldPrice(PurchasesModel.GetInfo<IPurchaseCoinInfo>(RespinPurchase).CoinCost);
        }

        protected void SpinAllItems()
        {
            RouletteSlotView slot;
            SavableItem item;
            float stopDelay = 0.5f;
            for (int i = 0; i < View.Slots.Count && i < LootItems.Count; i++)
            {
                slot = View.Slots[i];
                item = LootItems[i];
                List<SavableItem> spinItmes = TreasureLootModel.GetSpinItemsList(TreasureID, i, item);
                slot.SetItems(spinItmes, item);
                slot.StartSpin(stopDelay);
                stopDelay += 0.3f;
            }
            Spined = true;
        }

        protected void OnTakeAllHandler()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            TryTakeItems();
        }

        protected void OnDoubleVideo()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            PurchasesModel.Purchase(X2WatchPurchase,
                (result) => {
                    if (result == PurchaseResult.Successfully)
                    {
                        OnDoublePurchase();
                    }
                }
            );
        }

        protected void OnDoubleGold()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);

            var oceanLootX2Gold = PurchasesModel.GetInfo<IPurchaseCoinInfo>(X2GoldPurchase);
            if (oceanLootX2Gold.CoinCost > CoinsModel.Coins)
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
            else
            {
                PurchasesModel.Purchase(X2GoldPurchase,
                    (result) =>
                    {
                        if (result == PurchaseResult.Successfully)
                        {
                            OnDoublePurchase();
                        }
                    }
                );
            }
        }

        protected void OnDoublePurchase()
        {
            for (int i = 0; i < LootItems.Count; i++)
            {
                var item = LootItems[i];
                var slot = View.Slots[i];
                if (!slot.IsSpecial)
                {
                    item.Count *= 2;
                }
            }
            DoubleUsed = true;
            SetupDoubleButtons();
            SetupSlotsViews();
            TryTakeItems();
        }

        public virtual Vector3 DropChestPosition() => PlayerEventHandler.transform.position + PlayerEventHandler.transform.forward;
        protected virtual bool UseBigChestSize => false;

        protected virtual bool TryTakeItems()
        {
            Vector3 dropPoint = DropChestPosition();
            List<LootObject> lootObjects = DropContainerModel.DropContainer(dropPoint, 0.4f, GetDropItems(), UseBigChestSize);

            if(OpenLootBoxAfterDrop && lootObjects != null && lootObjects.Count > 0)
            {
                lootObjects[0].Open();
            }

            ViewsSystem.Hide(View);
            OnItemsTaken();

            return true;
        }

        private List<SavableItem> GetDropItems()
        {
            List<SavableItem> items = new List<SavableItem>();
            foreach(var item in LootItems)
            {
                if(!item.IsCanStackable)
                {
                    for(int i = 1; i <= item.Count; i++)
                    {
                        items.Add(new SavableItem(item.ItemData, 1));
                    }
                }
                else
                {
                    items.Add(item);
                }
            }
            return items;
        }

        protected virtual void OnItemsTaken() {
            ClearLootItems();
            UpdateSpawnTime();
            DoubleUsed = false;
            Spined = false;
        }


        protected void UpdateSpawnTime()
        {
            TimeSpawnTicks = GameTimeModel.RealTimeNowTick
                + GameTimeModel.GetTicks(RespawnSec);
        }

        protected void OnChangeLanguageHandler() => SetLocalization();

        protected void SetLocalization()
        {
            string take = LocalizationModel.GetString(LocalizationKeyID.LootBoxMenu_TakeBtn);
            string takeX2 = take + " x2";
            View.SetTakeButtonText(take);
            View.SetTakeX2ButtonText(takeX2);
        }

        protected void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        protected void OnRerollHandler(int cellIndex)
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            var respinGold = PurchasesModel.GetInfo<IPurchaseCoinInfo>(RespinPurchase);
            if (respinGold.CoinCost > CoinsModel.Coins)
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
            else
            {
                PurchasesModel.Purchase(RespinPurchase,
                    (result) => {
                        if (result == PurchaseResult.Successfully)
                        {
                            ReSpin(cellIndex);
                        }
                    }
                );
            }
        }

        protected void ReSpin(int slotIndex)
        {
            if (slotIndex >= LootItems.Count)
                return;

            var namesToRemove = LootItems.Select(i => i.Name).ToList();

            SavableItem item = TreasureLootModel.GetCellRespinLootItem(TreasureID, slotIndex, namesToRemove);
            SavableItem oldItem = LootItems[slotIndex];

            ReplaceLootItem(slotIndex, item);

            List<SavableItem> spinItems = TreasureLootModel.GetSpinItemsList(TreasureID, slotIndex, item);
            spinItems.Add(oldItem);
            View.Slots[slotIndex].SetItems(spinItems, LootItems[slotIndex], oldItem);
            View.Slots[slotIndex].StartSpin(0.5f);
        }
    }
}
