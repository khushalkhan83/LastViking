using Core;
using Core.Controllers;
using Extensions;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Collections;
using UltimateSurvival;
using UnityEngine;
using Game.Audio;

namespace Game.Controllers
{
    public class ConstructionInfoPopupViewController : ViewControllerBase<ConstructionInfoPopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }

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
        }

        protected override void Hide()
        {
            View.OnOkButton -= OnOkButton;
            View.OnBuildingButton -= OnBuildingButton;
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
        }


        private void OnChangeLanguageHandler() => SetLocalization();
        private void SetLocalization()
        {
            //View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_Title));
            //View.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_Description));
            View.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
        }

        private IEnumerator EnableOkButton() {
            yield return new WaitForSecondsRealtime(5f);
            View.SetOkButtonInteractable(true);
        }

        private void OnOkButton() {
            AudioSystem.PlayOnce(AudioID.PickUp);
            View.SetIsVisiblePopup(false);
            View.SetBuildingButtonInteractable(true);
        }

        public void OnBuildingButton() 
        {
            AudioSystem.PlayOnce(AudioID.PickUp);
            BuildingModeModel.BuildingActive = true;
            ViewsSystem.Hide(View);
        }
    }
}
