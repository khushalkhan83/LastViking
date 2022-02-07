using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Purchases;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class HotBarRepairingContainerViewController : ViewControllerBase<HotBarRepairingContainerView>
    {
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }

        protected HotRepairingView HotRepairingView { private set; get; }

        private bool IsRepearing(SavableItem item) => RepairingItemsModel.TryFindRepairItemInfo(item, out var info) && info.RemainingTime > 0;
        private bool CanBeRepeared(SavableItem item) => item.IsBroken() && !IsRepearing(item);

        protected override void Show()
        {
            HotBarModel.OnChangeEquipItem += OnChangeEquipCell;
            HotBarModel.OnChangeEquipCell += OnChangeEquipCell;
            RepairingItemsModel.OnChangeCount += OnChangeRepairingCount;

            UpdateRepairingStatus();
        }

        protected override void Hide()
        {
            HotBarModel.OnChangeEquipItem -= OnChangeEquipCell;
            HotBarModel.OnChangeEquipCell -= OnChangeEquipCell;
            RepairingItemsModel.OnChangeCount -= OnChangeRepairingCount;

            HideView();
        }

        private void OnChangeRepairingCount() => UpdateRepairingStatus();
        private void OnChangeEquipCell() => UpdateRepairingStatus();

        private void UpdateRepairingStatus()
        {
            var equippedCell = HotBarModel.EquipCell;
            if (equippedCell.IsHasItem && CanBeRepeared(equippedCell.Item))
            {
                HideView();
                ShowView(View.GetCell(HotBarModel.EquipCellId));
                UpdateItemRepairInfo(equippedCell.Item);
            }
            else
            {
                HideView();
            }
        }

        private void UpdateItemRepairInfo(SavableItem _item)
        {
            var repairCount = _item.GetProperty("Repair count");
            int repairIndex = repairCount.Int.Default - repairCount.Int.Current;

            PurchaseID purchaseID = GetRepairPurchaseId(_item, repairIndex);
            var info = PurchasesModel.GetInfo<IPurchaseCoinInfo>(purchaseID);
            if (info != null)
            {
                HotRepairingView.SetCountText(info.CoinCost.ToString());
            }
        }

        private PurchaseID GetRepairPurchaseId(SavableItem _item, int repairIndex)
        {
            if (_item.HasProperty("ItemLowQuality"))
                return RepairingItemsModel.RepairPurchaseLow(repairIndex);
            if (_item.HasProperty("ItemMidQuality"))
                return RepairingItemsModel.RepairPurchaseMid(repairIndex);
            if (_item.HasProperty("ItemHighQuality"))
                return RepairingItemsModel.RepairPurchaseHigh(repairIndex);

            return RepairingItemsModel.RepairPurchaseLow(repairIndex);
        }

        private void ShowView(Transform container)
        {
            if (HotRepairingView == null)
            {
                HotRepairingView = ViewsSystem.Show<HotRepairingView>(ViewConfigID.HotRepairingView, container);
                HotRepairingView.transform.localPosition = Vector3.zero;
                HotRepairingView.OnClick += OnClickRepair;
            }
        }

        private void HideView()
        {

            if (HotRepairingView != null)
            {
                HotRepairingView.OnClick -= OnClickRepair;
                ViewsSystem.Hide(HotRepairingView);
                HotRepairingView = null;
            }
        }

        private void OnClickRepair()
        {
            var item = HotBarModel.EquipCell.Item;
            var repairCount = item.GetProperty("Repair count");
            var repairIndex = repairCount.Int.Default - repairCount.Int.Current;

            PurchaseID purchaseID = GetRepairPurchaseId(item, repairIndex);
            PurchasesModel.Purchase(purchaseID, OnPurchaseRepairItem);
        }

        private void OnPurchaseRepairItem(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                RepairingItemsModel.RepairItemInstantly(HotBarModel.EquipCell.Item);
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }
    }
}
