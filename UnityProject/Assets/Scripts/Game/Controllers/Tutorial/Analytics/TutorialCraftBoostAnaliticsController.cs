using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialCraftBoostAnaliticsController : ITutorialCraftBoostAnaliticsController, IController
    {
        [Inject] public TutorialCraftBoostAnaliticsModel Model { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }

        private const int CraftTutorialStep = 5; // step 6 but index == 5 
        private const string TargetItemName = "tool_pickaxe_stone";

        void IController.Enable() 
        {
            HandleTutorialStep();
            
            TutorialModel.OnNextStep += HandleTutorialStep;
        }

        void IController.Start() { }

        void IController.Disable() 
        {
            TutorialModel.OnNextStep -= HandleTutorialStep;
            UnSubscribeFromCraftEvents();
        }

        private void HandleTutorialStep()
        {
            UnSubscribeFromCraftEvents();
            if(TutorialModel.Step != CraftTutorialStep) return;

            SubscribeToCraftEvents();
        }

        private void SubscribeToCraftEvents()
        {
            CraftModel.OnCraftedItem += OnCraftedItemHandler;
        }

        private void UnSubscribeFromCraftEvents()
        {
            CraftModel.OnCraftedItem -= OnCraftedItemHandler;
        }


        private void OnCraftedItemHandler(int itemId)
        {
            if(IsTargetItem(itemId))
            {
                var boostUsed = CraftModel.IsBoostNow;
                Model.SetItemCrafted(boostUsed);
                Debug.Log($"<color=orange> craftBoostUsed: {boostUsed.ToString()}  </color>");
            }
        }

        // copy paste logic from CraftItemTutorialStateBase
        private bool IsTargetItem(int itemId) => GetTargetItemID() == itemId;
        private int GetTargetItemID() => GetTargetItemData().Id;
        private ItemData GetTargetItemData() => ItemsDB.GetItem(TargetItemName);
    }
}