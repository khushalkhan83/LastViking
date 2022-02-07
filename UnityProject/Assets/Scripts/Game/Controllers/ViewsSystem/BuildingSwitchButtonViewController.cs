using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Objectives;
using Game.Views;

namespace Game.Controllers
{
    public class BuildingSwitchButtonViewController : ViewControllerBase<BuildingSwitchButtonView>
    {
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }

        protected override void Show()
        {
            View.OnClick += OnClickHandler;
            InputModel.OnInput.AddListener(PlayerActions.ConsturctionMode, OnClickHandler);
            BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled += OnBuildingDisabled;
            View.SetBuildingEnabled(BuildingModeModel.BuildingActive);
        }

        protected override void Hide()
        {
            View.OnClick -= OnClickHandler;
            InputModel.OnInput.RemoveListener(PlayerActions.ConsturctionMode, OnClickHandler);
        }

        private void OnClickHandler()
        {
            BuildingModeModel.BuildingActive = !BuildingModeModel.BuildingActive;
        }

        private void OnBuildingEnabled() {
            View.SetBuildingEnabled(BuildingModeModel.BuildingActive);
            AudioSystem.PlayOnce(AudioID.WindowOpen);
        }

        private void OnBuildingDisabled() {
            View.SetBuildingEnabled(BuildingModeModel.BuildingActive);
            AudioSystem.PlayOnce(AudioID.Button);
        }
    }
}
