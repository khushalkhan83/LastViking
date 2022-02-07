using Core;
using Game.Models;
using Game.Providers;
using Game.Views;

namespace Game.Controllers
{
    public class PlacePartButtonController : ViewEnableController<PlaceButtonView>, IPlacePartButtonController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public BuildingProcessModel BuildingProcessModel { get; private set; }
        [Inject] public SpritesProvider SpritesProvider { get; set; }
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; set; }

        private IDataViewController _data;

        public override void Enable()
        {
            _data = new PlaceButtonData(
                SpritesProvider[SpriteID.PlacePartButtonActive],
                SpritesProvider[SpriteID.PlacePartButtonDefault]);

            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            BuildingProcessModel.OnBuildingStarted += OnBuildingEnabled;
            BuildingProcessModel.OnBuildingEnded += OnBuildingDisabled;

            UpdateViewVisible();
        }

        public override void Start()
        {
        }

        public override void Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            BuildingProcessModel.OnBuildingStarted -= OnBuildingEnabled;
            BuildingProcessModel.OnBuildingEnded -= OnBuildingDisabled;

            Hide();
        }

        private void OnBuildingDisabled() => UpdateViewVisible();
        private void OnBuildingEnabled() => UpdateViewVisible();

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();

        public override ViewConfigID ViewConfigID => ViewConfigID.PlacePartButton;
        public override IDataViewController Data => _data;

        public override bool IsCanShow => !PlayerHealthModel.IsDead && BuildingProcessModel.IsBuildingEnabled && !ReplaceBuildingModel.ConstructionReplaceActive;
    }
}
