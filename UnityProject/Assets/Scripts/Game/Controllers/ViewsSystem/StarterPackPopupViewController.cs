using Core;
using Core.Controllers;
using Extensions;
using Game.Models;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using static Game.Models.StarterPackModel;

namespace Game.Controllers
{
    public class StarterPackPopupViewController : ViewControllerBase<StarterPackView>
    {
        [Inject] public StarterPackModel StarterPackModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public PurchasersStoreModel PurchasersStoreModel { get; private set; }
        [Inject] public RealTimeModel RealTimeModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }


        protected override void Show()
        {
            View.OnClose += OnClose;

            GameUpdateModel.OnUpdate += OnUpdate;

            RealTimeModel.OnTimeReady += OnTimeReady;
            RealTimeModel.OnTimeError += OnTimeError;
            PurchasersStoreModel.Initialized += HandlePurchasersStoreModelInitialized;
            UpdateViews();
        }

        protected override void Hide()
        {
            View.OnBuySurvival -= OnBuySurvival;
            View.OnBuyDominant -= OnBuyDominant;
            View.OnClose -= OnClose;
            GameUpdateModel.OnUpdate -= OnUpdate;
            RealTimeModel.OnTimeReady -= OnTimeReady;
            RealTimeModel.OnTimeError -= OnTimeError;
            PurchasersStoreModel.Initialized -= HandlePurchasersStoreModelInitialized;
        }

        private void SubscribeButtons()
        {
            View.OnBuySurvival += OnBuySurvival;
            View.OnBuyDominant += OnBuyDominant;
        }

        private void UnsubscribeButtons()
        {
            View.OnBuySurvival -= OnBuySurvival;
            View.OnBuyDominant -= OnBuyDominant;
        }

        private void OnBuySurvival()
        {
            if (!StarterPackModel.IsPackAvailable) return;
            PurchasesModel.Purchase(PurchaseID.SurvivalPack, OnPurchasedSurvivalHandler);
        }

        private void OnBuyDominant()
        {
            if (!StarterPackModel.IsPackAvailable) return;
            PurchasesModel.Purchase(PurchaseID.DominationPack, OnPurchasedDominationHandler);
        }

        private void OnPurchasedSurvivalHandler(PurchaseResult purchaseResult)
        {
            ViewsSystem.Hide(View);
        }

        private void OnPurchasedDominationHandler(PurchaseResult purchaseResult)
        {
            ViewsSystem.Hide(View);
        }


        private void OnTimeError(string obj) => UpdateButtons();
        private void OnTimeReady() => UpdateButtons();

        private void OnClose() => ViewsSystem.Hide(View);

        private void HandlePurchasersStoreModelInitialized() => UpdateButtons();

        private void OnUpdate()
        {
            string minSec = LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_MinSec);
            string hourMin = LocalizationModel.GetString(LocalizationKeyID.ObjectivesMenu_HourMin);
            string timeText = StarterPackModel.RemainOfferTime.TimeToString(minSec, hourMin);
            View.SetRemainOfferTimeText(timeText);
        }

        private void UpdateViews()
        {
            UpdateSurvivalViews();
            UpdateDominationViews();
            UpdateButtons();
        }

        private void HideButtons()
        {
            UnsubscribeButtons();
        }

        private void ShowButtons()
        {
            bool isReady = RealTimeModel.isReady && PurchasersStoreModel.IsReady;
            if (isReady)
                SubscribeButtons();
            

            View.SurvivalButton.SetIsVisible(isReady);
            View.DominationButton.SetIsVisible(isReady);
            View.SetSurvivalButtonText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.SurvivalPack).PriceString);
            View.SetDominatioButtonText(PurchasesModel.GetInfo<IPurchaseStoreInfo>(PurchaseID.DominationPack).PriceString);
        }

        private void UpdateButtons()
        {
            HideButtons();
            ShowButtons();
        }

        private void UpdateSurvivalViews()
        {
            int i = 0;
            foreach (var cell in View.SurvivalCells)
            {
                var item = StarterPackModel.SurvivalItems[i];
                cell.SetData(GetData(item));
                i++;
            }
        }

        private void UpdateDominationViews()
        {
            int i = 0;
            foreach (var cell in View.DominationCells)
            {
                var item = StarterPackModel.DominationItems[i];
                cell.SetData(GetData(item));
                i++;
            }
        }

        private ResourceCellData GetData(ItemSettings item)
        {
            var dbItem = ItemsDB.GetItem(item.ItemName);

            return new ResourceCellData
            {
                Icon = dbItem.Icon,
                Message = item.Count > 1 ? item.Count.ToString() : String.Empty,
                IsSelected = true,
            };
        }

        private IEnumerable<SavableItem> GetSavableItems(ItemSettings[] itemSettings)
        {
            List<SavableItem> items = new List<SavableItem>();
            foreach (var item in itemSettings)
            {
                var data = ItemsDB.GetItem(item.ItemName);
                var count = item.Count;

                while (count > data.StackSize)
                {
                    items.Add(new SavableItem(data, data.StackSize));
                    count -= data.StackSize;
                }
                items.Add(new SavableItem(data, count));
            }

            return items;
        }
    }
}