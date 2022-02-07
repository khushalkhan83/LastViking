using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class HotBarController : IHotBarController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; private set; }

        IView View { get; set; }

        void IController.Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled += OnBuildingDisabled;
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged += OnReplaceActiveChanged;
            ReplaceBuildingModel.OnReplaceConstructionActiveChanged += OnReplaceActiveChanged;

            UpdateViewVisible();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            BuildingModeModel.BuildingEnabled -= OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled -= OnBuildingDisabled;
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged -= OnReplaceActiveChanged;
            ReplaceBuildingModel.OnReplaceConstructionActiveChanged -= OnReplaceActiveChanged;

            HideView();
        }

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();
        private void OnBuildingEnabled() => UpdateViewVisible();
        private void OnBuildingDisabled() => UpdateViewVisible();
        private void OnReplaceActiveChanged() => UpdateViewVisible();

        private bool IsCanShow => !PlayerHealthModel.IsDead && !BuildingModeModel.BuildingActive && !ReplaceBuildingModel.BuildingReplaceActive && !ReplaceBuildingModel.ConstructionReplaceActive;

        private void UpdateViewVisible()
        {
            if (IsCanShow)
            {
                ShowView();
            }
            else
            {
                HideView();
            }
        }

        private void ShowView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<HotBarView>(ViewConfigID.HotBar);
                View.OnHide += OnHideHandler;
            }
        }

        private void HideView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }
    }
}
