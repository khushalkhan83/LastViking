using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class CraftInteractViewController : ViewControllerBase<CraftInteractView>
    {
        [Inject] public CraftInteractViewModel CraftInteractViewModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDb { get; private set; }

        protected Coroutine SetCraftCountProcess { get; private set; }

        protected override void Show()
        {
            View.OnClick += OnClickHandler;

            CraftInteractViewModel.OnStartPulse += OnStartPulseHandler;
            CraftInteractViewModel.OnStopPulse += OnStopPulseHandler;
            CraftInteractViewModel.OnStartCalculate += OnStartCalculateHandler;
            CraftInteractViewModel.OnStopCalculate += OnStopCalculateHandler;
            CraftInteractViewModel.OnChangeGear += OnChangeGearHandler;

            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellInventoryHandler;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHotBarHandler;
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalHandler;
            CraftModel.OnUnlockedItem += OnUnlockedItemHandler;

            View.SetIsVisibleGear(false);

            if (TutorialModel.IsTutorialNow && TutorialModel.ObjectiveIDCurrent == ObjectiveID.TutorialCraftHatchet)
            {
                CraftInteractViewModel.StartPulse();
            }
            else
            {
                if (CraftInteractViewModel.IsCalculateProcess)
                {
                    StartSetCraftCountProcess();
                }
            }
        }

        protected override void Hide()
        {
            CraftInteractViewModel.StopCalculateProcess();

            View.OnClick -= OnClickHandler;

            CraftInteractViewModel.OnStartPulse -= OnStartPulseHandler;
            CraftInteractViewModel.OnStopPulse -= OnStopPulseHandler;
            CraftInteractViewModel.OnStartCalculate -= OnStartCalculateHandler;
            CraftInteractViewModel.OnStopCalculate -= OnStopCalculateHandler;
            CraftInteractViewModel.OnChangeGear -= OnChangeGearHandler;

            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellInventoryHandler;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHotBarHandler;

            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalHandler;
            CraftModel.OnUnlockedItem -= OnUnlockedItemHandler;
        }

        private void OnChangeGearHandler(int count)
        {
            var isVisible = count > 0;

            View.SetIsVisibleGear(isVisible);

            if (isVisible)
            {
                View.SetTextCount(count.ToString());
                View.PlayRotateGear();
            }
        }

        private void OnStopCalculateHandler() => StopSetCraftCountProcess();
        private void OnStartCalculateHandler() => StartSetCraftCountProcess();
        private void OnStopPulseHandler() => View.PlayDefault();
        private void OnStartPulseHandler() => View.PlayPulse();
        private void OnUnlockedItemHandler(int itemId) => CraftInteractViewModel.ChangeResourceCount();
        private void OnChangeCellHotBarHandler(CellModel cell) => CraftInteractViewModel.ChangeResourceCount();
        private void OnChangeCellInventoryHandler(CellModel cell) => CraftInteractViewModel.ChangeResourceCount();
        private void OnRevivalHandler() => CraftInteractViewModel.ApplyChanges(0);

        private void OnClickHandler()
        {
            CraftInteractViewModel.Click();
            View.SetIsInteractable(false);

            var craftView = ViewsSystem.Show<CraftView>(ViewConfigID.Craft);
            craftView.OnHide += OnHideCraftView;
        }

        private void OnHideCraftView(IView view)
        {
            view.OnHide -= OnHideCraftView;
            CraftInteractViewModel.ChangeResourceCount();
            View.SetIsInteractable(true);
        }

        private void StartSetCraftCountProcess()
        {
            if (SetCraftCountProcess == null)
            {
                SetCraftCountProcess = StartCoroutine(CheckCraftsCountProcess());
            }
        }

        private void StopSetCraftCountProcess()
        {
            if (SetCraftCountProcess != null)
            {
                StopCoroutine(SetCraftCountProcess);
                SetCraftCountProcess = null;
            }
        }

        private IEnumerator CheckCraftsCountProcess()
        {
            var items = ItemsDb.ItemDatabase.Categories.SelectMany(x => x.Items);
            var craftCount = 0;

            foreach (var item in items)
            {
                yield return null;

                if (IsCanCraft(item) && IsHasResources(item.Recipe.RequiredItems))
                {
                    ++craftCount;
                }
            }
            CraftInteractViewModel.ApplyChanges(craftCount);
        }

        private bool IsHasPlayerResources(RequiredItem item)
        {
            var itemData = ItemsDb.GetItem(item.Name);
            var count = InventoryModel.ItemsContainer.GetItemsCount(itemData.Id);
            if (count < item.Amount)
            {
                count += HotBarModel.ItemsContainer.GetItemsCount(itemData.Id);

                if (count < item.Amount)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsHasResources(IEnumerable<RequiredItem> items) => items.All(IsHasPlayerResources);
        private bool IsCanCraft(ItemData item) => item.IsCraftable && (!item.IsUnlockable || CraftModel.IsUnlocked(item.Id));
    }
}
