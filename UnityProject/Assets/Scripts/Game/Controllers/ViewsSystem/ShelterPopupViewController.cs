using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Controllers
{
    //[REFACTOR]
    public class ShelterPopupViewController : ViewControllerBase<ShelterPopupView>
    {
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ShelterUpgradeModel ShelterUpgradeModel { get; private set; }
        [Inject] public QuestsModel QuestsModel { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public TooltipModel TooltipModel { get; private set; }

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        protected AttackWarningPopupView AttackWarningPopupView { get; private set; }
        private bool EnemiesAttackWillStartOnUpgrade => ShelterUpgradeModel.EnemiesAttackWillStartOnUpgrade;
        private bool IgnorePriceForUpgrade => EditorGameSettings.IgnoreItemsPrice;
        private ShelterCost[] costs;

        private int QuestCellIndex => costs.Length + 1;

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);

            var isVisibleBuyPanel = ShelterUpgradeModel.CanBeUpgraded;

            LocalizationModel.OnChangeLanguage += SetLocalization;
            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCell;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCell;
            View.OnClose += OnCloseHandler;
            InputModel.OnInput.AddListener(PlayerActions.Cancel, OnCloseHandler);


            bool isEnoughResources = false;
            if (SheltersModel.ShelterActive == ShelterModelID.None)
            {
                isEnoughResources = SetCostUpgrade(SheltersModel.GetComponentsInChildren<ShelterModel>().First(x => x.ShelterID == ShelterModelID.Ship).CostBuy.Costs);
            }
            else if (!SheltersModel.ShelterModel.IsMaxLevel)
            {
                isEnoughResources = SetCostUpgrade(SheltersModel.ShelterModel.CostUpgradeCurrent.Costs);
            }

            if(IgnorePriceForUpgrade) isEnoughResources = true;

            bool showWarningMessage = isEnoughResources && EnemiesAttackWillStartOnUpgrade;

            View.SetIsVisibleSkeletonAttackPanel(showWarningMessage);
            View.SetIsVisiableSelectedResourceDescription(!showWarningMessage);

            var isCanBuy = ShelterUpgradeModel.CanBeUpgraded && isEnoughResources;

            if (isCanBuy)
            {
                View.OnBuy += OnBuyHandler;
            }
            View.SetIsVisibleBuyButton(isCanBuy);

            SetLocalization();

            SelectResourceCell(0);
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;

            View.OnClose -= OnCloseHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.Cancel, OnCloseHandler);
            View.OnBuy -= OnBuyHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCell;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCell;

            foreach (var item in View.ResourceCells)
            {
                item.OnPointDown_ -= OnResourceCellClick;
            }

            View.QuestItemCells.OnPointDown_ -= OnQuestItemCellClick;

            HideAttackWarningPopupView();
        }

        private bool SetCostUpgrade(ShelterCost[] costs)
        {
            this.costs = costs;
            var result = true;

            int i = 0;
            for(; i < costs.Count() && i < View.ResourceCells.Length; i++)
            {
                ShelterCost item = costs[i];
                var itemData = ItemsDB.GetItem(item.Name);
                var countCurrent = InventoryModel.ItemsContainer.GetItemsCount(itemData.Id) + HotBarModel.ItemsContainer.GetItemsCount(itemData.Id);
                if (item.Count > countCurrent)
                {
                    result = false;
                }

                var cellData = GetCellData(item, itemData, countCurrent);
                View.ResourceCells[i].gameObject.SetActive(true);
                View.ResourceCells[i].SetData(cellData);
                View.ResourceCells[i].OnPointDown_ += OnResourceCellClick;
            }

            for(; i < View.ResourceCells.Length; i++)
            {
                 View.ResourceCells[i].gameObject.SetActive(false);
            }

            if(ShelterUpgradeModel.NeedQuestItem)
            {
                View.QuestItemCells.gameObject.SetActive(true);
                View.QuestItemCells.SetData(GetQuestCellData());
                View.QuestItemCells.OnPointDown_ += OnQuestItemCellClick;
            }
            else
            {
                View.QuestItemCells.gameObject.SetActive(false);
            }

            return result;
        }

        private void OnResourceCellClick(ResourceCellView view, PointerEventData eventData)
        {
            var viewIndex = View.ResourceCells.ToList().IndexOf(view);
            SelectResourceCell(viewIndex);
        }

        private void OnQuestItemCellClick(ResourceCellView view, PointerEventData eventData)
        {
            var viewIndex = QuestCellIndex;
            SelectResourceCell(viewIndex);
        }

        private ResourceCellView selectedView;
        private void SelectResourceCell(int index)
        {
            if(selectedView != null)
                selectedView.SetIsVisibleBorderSelection(false);

            selectedView = null;

            if(View.ResourceCells.IndexIsValid(index) && costs.IndexIsValid(index))
            {
                var view = View.ResourceCells[index];
                view.SetIsVisibleBorderSelection(true);
                selectedView = view;
                
                ShelterCost cost = costs[index];
                TooltipModel.TryGetLocalizedItemTooltipText(cost.Name, out string answer);
                View.SetSelectedResourceDescription(answer);
            }
            else if(IsQuestItemIndex())
            {
                var view = View.QuestItemCells;
                view.SetIsVisibleBorderSelection(true);
                selectedView = view;

                TooltipModel.TryGetLocalizedQuestItemTooltipText(QuestsModel.QuestItemData, out string answer);
                View.SetSelectedResourceDescription(answer);
            }

            bool IsQuestItemIndex() => ShelterUpgradeModel.NeedQuestItem && QuestCellIndex == index;
        }

        private ResourceCellData GetCellData(ShelterCost itemCost, ItemData itemData, int countCurrent)
        {
            return new ResourceCellData
            {
                Icon = itemData.Icon,
                Message = countCurrent + "/" + itemCost.Count,
                IsActive = countCurrent >= itemCost.Count,
                ItemRarity = itemData.ItemRarity,
                IsComponent = itemData.Category == "Components",
            };
        }

        private ResourceCellData GetQuestCellData()
        {
            bool isCanBuy = ShelterUpgradeModel.CanBeUpgraded;
            return new ResourceCellData
            {
                Icon = QuestsModel.QuestItemData.ItemIcon,
                Message = isCanBuy? "1/1" : "0/1",
                IsActive = isCanBuy,
            };
        }

        private void OnChangeCell(CellModel cell)
        {
            Hide();
            Show();
        }

        private void OnBuyHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            if(!EnemiesAttackWillStartOnUpgrade)
            {            
                UpgradeShelter();
            }
            else
            {
               ShowAttackWarningPopupView();
            }
        }

        private void ShowAttackWarningPopupView()
        {
            AttackWarningPopupView = ViewsSystem.Show<AttackWarningPopupView>(ViewConfigID.AttackWarningPopup);
            AttackWarningPopupView.OnClose += OnCloseAttackWarningHandler;
            AttackWarningPopupView.OnApply += OnApplyAttackWarningHandler;
            AttackWarningPopupView.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_AttackWarningPopup));
            AttackWarningPopupView.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
            AttackWarningPopupView.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
        }

        private void OnCloseAttackWarningHandler()
        { 
            AudioSystem.PlayOnce(AudioID.Button);
            HideAttackWarningPopupView();
        }

        private void OnApplyAttackWarningHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            HideAttackWarningPopupView();
            UpgradeShelter();
        }

        private void HideAttackWarningPopupView()
        {
            if(AttackWarningPopupView != null)
            {
                AttackWarningPopupView.OnClose -= OnCloseAttackWarningHandler;
                AttackWarningPopupView.OnApply -= OnApplyAttackWarningHandler;
                ViewsSystem.Hide(AttackWarningPopupView);
                AttackWarningPopupView = null;
            }
        }

        private void UpgradeShelter()
        {
            ShelterUpgradeModel.StartUpgrade();
            Close();
        }

        private void OnCloseHandler()
        {
            Close();
        }

        private void Close()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            int shipLevel = 0;
            if (SheltersModel.ShelterActive != ShelterModelID.None)
            {
                shipLevel = SheltersModel.ShelterModel.Level + 1;
            }

            View.SetShipLevelText(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_Level) + " " + shipLevel);
            View.SetTextDescriptionName(GetTextDescritptionName());
            View.SetTextDescription(QuestsModel.StageDescription);
            View.SetAttackWarningText(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_AttackWarning));

            if (SheltersModel.ShelterActive == ShelterModelID.None)
            {
                View.SetTextBuildButton(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_BuildButton));
            }
            else
            {
                View.SetTextBuildButton(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_LevelUpBtn));
            }

            string GetTextDescritptionName()
            {
                var questItem = QuestsModel.QuestItemData;

                if(questItem != null)
                    return LocalizationModel.GetString(questItem.LocalizationKeyID);
                else
                    return "";
            }
        }
    }
}
