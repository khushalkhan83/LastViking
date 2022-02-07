using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class FishingPullViewController : ViewControllerBase<FishingPullView>
    {
        [Inject] public FishingModel fishingModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            fishingModel.OnTension += SetTenstion;
            View.OnPointer += OnPointer;
            LocalizationModel.OnChangeLanguage += OnChangeLocalization;

            SetTenstion(fishingModel.initPosition);
            OnChangeLocalization();
            View.SetButtonPressed(false);
        }

        void OnChangeLocalization()
        {
            View.SetHint(LocalizationModel.GetString(LocalizationKeyID.fishing_PressHint));
        }

        protected override void Hide()
        {
            fishingModel.OnTension -= SetTenstion;
            View.OnPointer -= OnPointer;
            LocalizationModel.OnChangeLanguage -= OnChangeLocalization;
        }

        private void OnPointer(bool isHold)
        {
            fishingModel.Pull(isHold);
            View.SetButtonPressed(isHold);
        }

        void SetTenstion(float t)
        {
            View.SetTension(t);
        }
    }

}
