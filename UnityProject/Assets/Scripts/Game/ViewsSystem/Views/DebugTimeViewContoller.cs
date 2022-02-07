using System;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class DebugTimeViewController : ViewControllerBase<DebugTimeView>
    {
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public ShelterModelsProvider ShelterModelsProvider { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public DebugTimeModel DebugTimeModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }

        public ShelterModel ShelterModel => ShelterModelsProvider[ShelterModelID.Ship];
        EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;
        
        private void Start() 
        {
            if(EditorGameSettings.godModeOnStart)
            {
                OnChangeGodModeHandler(true);
            }
        }
        protected override void Show()
        {
            View.OnChangeGodMode += OnChangeGodModeHandler;
            View.OnAdjustHealth += OnAddHealthHandler;
            View.OnAdjustHunger += OnAddHungerHandler;
            View.OnAdjustWater += OnAddWaterHandler;
            View.OnKillPlayer += OnPlayerKillHandler;
            View.OnAddCoins += OnAddCoinHandler;
            View.OnBuildShelter += OnBuildShelter;
            View.OnKillShelter += OnKillShelter;
            View.OnSwitchControllerActions += OnSwitchControllerActionsHandler;
            View.OnSkipTutorial += OnSkipTutorialHandler;
            View.OnNextTier += OnNextTierHandler;
            View.OnTimeNormal += OnTimeNormalHandler;
            View.OnTimeFaster += OnTimeFasterHandler;
        }

        protected override void Hide()
        {
            View.OnChangeGodMode -= OnChangeGodModeHandler;
            View.OnAdjustHealth -= OnAddHealthHandler;
            View.OnAdjustHunger -= OnAddHungerHandler;
            View.OnAdjustWater -= OnAddWaterHandler;
            View.OnKillPlayer -= OnPlayerKillHandler;
            View.OnAddCoins -= OnAddCoinHandler;
            View.OnBuildShelter -= OnBuildShelter;
            View.OnKillShelter -= OnKillShelter;
            View.OnSwitchControllerActions -= OnSwitchControllerActionsHandler;
            View.OnSkipTutorial -= OnSkipTutorialHandler;
            View.OnNextTier -= OnNextTierHandler;
            View.OnTimeFaster -= OnTimeFasterHandler;
            View.OnTimeNormal -= OnTimeNormalHandler;
        }

        private void OnTimeFasterHandler()
        {
            Time.timeScale++;
        }

        private void OnTimeNormalHandler()
        {
            Time.timeScale = 1;
        }

        private void OnSkipTutorialHandler() => TutorialModel.SkipTutorial();

        private void OnNextTierHandler() => PlayerObjectivesModel.NextTier();

        private void OnChangeGodModeHandler(bool IsGodMode)
        {
            PlayerHealthModel.CantRecieveDamage(IsGodMode);
            PlayerWaterModel.CantThirst(IsGodMode);
            PlayerFoodModel.CantHunger(IsGodMode);
            PlayerStaminaModel.CantLoseStamina(IsGodMode);

            DebugTimeModel.SetIsGodMode(IsGodMode);
        }

        private void OnAddHealthHandler() => PlayerHealthModel.RefillHealth();

        private void OnAddHungerHandler() => PlayerFoodModel.RefillFood();

        private void OnAddWaterHandler() => PlayerWaterModel.RefillWater();

        private void OnPlayerKillHandler() => PlayerHealthModel.KillPlayer();

        private void OnAddCoinHandler()
        {
            CoinsModel.Adjust(750);
            BluePrintsModel.Adjust(750);
        }

        private void OnSwitchControllerActionsHandler(bool IsKeyboard)
        {
            // if (Application.platform != RuntimePlatform.Android || Application.platform != RuntimePlatform.IPhonePlayer)
            // {
            //     View.KeyBoardControllerActions.enabled = !IsKeyboard;
            // }
        }

        private void OnBuildShelter()
        {
            if (SheltersModel.ShelterActive == ShelterModelID.None)
            {
                SheltersModel.SetShelter(ShelterModel);

                SheltersModel.ShelterModel.Buy(GameTimeModel.Ticks);
                SheltersModel.ShelterModel.Activate();
            }
        }

        private void OnKillShelter()
        {
            if (SheltersModel.ShelterActive != ShelterModelID.None)
            {
                SheltersModel.ShelterModel.Death();
            }
        }
    }
}

