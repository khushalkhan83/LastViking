using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class ExpandInventoryController : IExpandInventoryController, IController
    {
        /* �����������^
         * 
         * RemoteSettingsModel.Get<InventoryRemoteSettings>(RemoteSettingID.Inventory);
         * PurchaseID.InventoryExpandWatch
         * UnlockSlotsGoldView View, config Id
         * 
         * PurchaseID.PlayerInventoryExpandGold)
         * KeyID.InventoryMenu_ExpandBtn));
         * InventoryModel
         * InventoryViewModel
         */

        [Inject] public ExpandInventoryModel ExpandInventoryModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InventoryViewModel InventoryViewModel { get; private set; }
        [Inject] public RemoteSettingsModel RemoteSettingsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }

        void IController.Enable() 
        {
            ExpandInventoryModel.OnShowExpandInventoryButton += ShowExpandInventoryButton;
            ExpandInventoryModel.OnHideExpandInventoryButton += HideExpandInventoryButton;
            ExpandInventoryModel.OnExpandInventory += OnExpandInventoryHandler;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            ExpandInventoryModel.OnShowExpandInventoryButton -= ShowExpandInventoryButton;
            ExpandInventoryModel.OnHideExpandInventoryButton -= HideExpandInventoryButton;
            ExpandInventoryModel.OnExpandInventory -= OnExpandInventoryHandler;
        }

        protected InventoryRemoteSettings RemoteSettingsInventory => RemoteSettingsModel.Get<InventoryRemoteSettings>(RemoteSettingID.Inventory);

        protected UnlockSlotsGoldView UnlockSlotsGoldView { get; private set; }
        protected UnlockSlotsWatchView UnlockSlotsWatchView { get; private set; }

        protected bool GetIsExpandInventoryButtonWatch(IWatchPurchase purchase) => !RemoteSettingsInventory.IsBuySlotsGold && (purchase.IsCanPurchase || NetworkModel.IsHasConnection);

        protected bool IsExpandInventoryButtonWatch => GetIsExpandInventoryButtonWatch(WatchPurchaseInventoryExpand);

        protected IWatchPurchase WatchPurchaseInventoryExpand => PurchasesModel.GetInfo<IWatchPurchase>(PurchaseID.InventoryExpandWatch);

        private void HideExpandInventoryButton()
        {
            HideExpandButtonGold();
            HideExpandButtonWatch();
        }

        private void HideExpandButtonWatch()
        {
            if (UnlockSlotsWatchView != null)
            {
                UnlockSlotsWatchView.OnUnlockOnce -= OnExpandInventoryHandler;
                UnlockSlotsWatchView.OnUnlockForever -= OnExpandForeverInventoryHandler;
                ViewsSystem.Hide(UnlockSlotsWatchView);
                UnlockSlotsWatchView = null;
            }
        }

        private void HideExpandButtonGold()
        {
            if (UnlockSlotsGoldView != null)
            {
                UnlockSlotsGoldView.OnUnlockOnce -= OnExpandInventoryHandler;
                UnlockSlotsGoldView.OnUnlockForever -= OnExpandForeverInventoryHandler;
                ViewsSystem.Hide(UnlockSlotsGoldView);
                UnlockSlotsGoldView = null;
            }
        }

        private void UpdateExpandInventoryButton()
        {
            HideExpandInventoryButton();
            ShowExpandInventoryButton();
        }

        private void ShowExpandInventoryButton()
        {
            if (!InventoryViewModel.IsMaxExpandLevel)
            {
                var purchase = WatchPurchaseInventoryExpand;
                if (GetIsExpandInventoryButtonWatch(purchase))
                {
                    purchase.Prepere();
                    ShowExpandInventoryButtonWatch();
                }
                else
                {
                    ShowExpandInventoryButtonGold();
                }
            }
        }

        private void ShowExpandInventoryButtonGold()
        {
            UnlockSlotsGoldView = ViewsSystem.Show<UnlockSlotsGoldView>(ViewConfigID.UnlockSlotsGold, ExpandInventoryModel.ButtonContainer);
            UnlockSlotsGoldView.transform.localPosition = Vector3.zero;

            UnlockSlotsGoldView.OnUnlockOnce += OnExpandInventoryHandler;
            UnlockSlotsGoldView.OnUnlockForever += OnExpandForeverInventoryHandler;

            UnlockSlotsGoldView.SetTextExpandOnceCoin(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.PlayerInventoryExpandGold).CoinCost.ToString());
            UnlockSlotsGoldView.SetTextExpandForeverCoin(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.InventoryExpandForeverGold).CoinCost.ToString());
            UnlockSlotsGoldView.SetTextExpandOnceButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandOnceBtn));
            UnlockSlotsGoldView.SetTextExpandForeverButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandBtn));
        }

        private void ShowExpandInventoryButtonWatch()
        {
            UnlockSlotsWatchView = ViewsSystem.Show<UnlockSlotsWatchView>(ViewConfigID.UnlockSlotsWatch, ExpandInventoryModel.ButtonContainer);
            UnlockSlotsWatchView.transform.localPosition = Vector3.zero;

            UnlockSlotsWatchView.OnUnlockOnce += OnExpandInventoryHandler;
            UnlockSlotsWatchView.OnUnlockForever += OnExpandForeverInventoryHandler;

            UnlockSlotsWatchView.SetTextExpandButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandOnceBtn));
            UnlockSlotsWatchView.SetTextExpandForeverButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandBtn));
            UnlockSlotsWatchView.SetTextExpandForeverCoin(PurchasesModel.GetInfo<IPurchaseCoinInfo>(PurchaseID.InventoryExpandForeverGold).CoinCost.ToString());
        }


        // Split in 2 ?


        private void OnExpandInventoryHandler()
        {
            if (IsExpandInventoryButtonWatch)
            {
                PurchasesModel.Purchase(PurchaseID.InventoryExpandWatch, OnPurchaseInventoryExpandWatch);
            }
            else
            {
                PurchasesModel.Purchase(PurchaseID.PlayerInventoryExpandGold, OnPurchaseInventoryExpandGold);
            }
        }

        private void OnExpandForeverInventoryHandler() => PurchasesModel.Purchase(PurchaseID.InventoryExpandForeverGold, OnPurchaseInventoryExpandForeverGold);

        private void OnPurchaseInventoryExpandWatch(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandPlayerInventory();
                ExpandInventoryModel.TriggerExpandedCallback();
            }
            else
            {
                UpdateExpandInventoryButton();
                ExpandInventoryModel.TriggerNotExpandedCallback();
            }
        }

        private void OnPurchaseInventoryExpandGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandPlayerInventory();
                ExpandInventoryModel.TriggerExpandedCallback();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
                ExpandInventoryModel.TriggerNotExpandedCallback();
            }
        }

        private void OnPurchaseInventoryExpandForeverGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                ExpandPlayerInventoryForever();
                ExpandInventoryModel.TriggerExpandedCallback();
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
                ExpandInventoryModel.TriggerNotExpandedCallback();
            }
        }

        private void ExpandPlayerInventoryForever()
        {
            InventoryModel.ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);
            InventoryViewModel.ExpandCellsForever();
        }

        private void ExpandPlayerInventory()
        {
            InventoryModel.ItemsContainer.AddCells(InventoryViewModel.CellsExpanded);
            InventoryViewModel.ExpandCellsOnce();
        }

        private void SetLocalization()
        {
            UnlockSlotsGoldView?.SetTextExpandOnceButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandOnceBtn));
            UnlockSlotsGoldView?.SetTextExpandForeverButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandBtn));
            UnlockSlotsWatchView?.SetTextExpandButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandOnceBtn));
            UnlockSlotsWatchView?.SetTextExpandForeverButton(LocalizationModel.GetString(LocalizationKeyID.InventoryMenu_ExpandBtn));
        }
    }
}
