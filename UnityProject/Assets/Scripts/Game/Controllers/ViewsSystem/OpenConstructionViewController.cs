using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using System.Collections;
using UnityEngine;
using Game.Audio;

namespace Game.Controllers
{
    // based on ConstructionInfoPopupViewController
    public class OpenConstructionViewController : ViewControllerBase<ConstructionInfoPopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel {get;private set;}


        protected override void Show()
        {
            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;
            View.OnOkButton += OnOkButton;
            View.OnBuildingButton += OnBuildingButton;
            View.SetIsVisiblePopup(true);
            View.SetOkButtonInteractable(false);
            View.SetBuildingButtonInteractable(false);
            StartCoroutine(EnableOkButton());
            SetLocalization();

            OnOkButton();
            PlayerMovementModel.SetBlockPlayerMovement(true);

            // GameTimeModel.ScaleSave();
            // GameTimeModel.ScaleStop();
        }

        protected override void Hide()
        {
            View.OnOkButton -= OnOkButton;
            View.OnBuildingButton -= OnBuildingButton;
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
            PlayerMovementModel.SetBlockPlayerMovement(false);

            // GameTimeModel.ScaleRestore();
        }


        private void OnChangeLanguageHandler() => SetLocalization();
        private void SetLocalization()
        {
            View.SetTextBuilding(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_ConstructionButton));
            View.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_UseConstruction));
        }

        private IEnumerator EnableOkButton()
        {
            yield return new WaitForSecondsRealtime(5f);
            View.SetOkButtonInteractable(true);
        }

        private void OnOkButton()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            View.SetIsVisiblePopup(false);
            View.SetBuildingButtonInteractable(true);
        }

        public void OnBuildingButton()
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            BuildingModeModel.BuildingActive = true;
            BuildingModeModel.HighlightPlaceButton(true);
            BuildingModeModel.HideSwitchButton = false;
            ViewsSystem.Hide(View);
        }
    }
}
