using UnityEngine;
using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class BuildingSwitchButtonController : ViewEnableController<BuildingSwitchButtonView>, IBuildingSwitchButtonController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }

        public override ViewConfigID ViewConfigID => ViewConfigID.BuildingSwitchButton;
        public override bool IsCanShow => !PlayerHealthModel.IsDead && !BuildingModeModel.HideSwitchButton;

        public override void Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            BuildingModeModel.OnHideSwitchButtonChanged += OnHideSwitchButtonChanged;

            UpdateViewVisible();
        }

        public override void Start()
        {

        }

        public override void Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            BuildingModeModel.OnHideSwitchButtonChanged -= OnHideSwitchButtonChanged;
            Hide();
        }

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();
        private void OnHideSwitchButtonChanged() => UpdateViewVisible();

       
    }
}
