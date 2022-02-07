using Core;
using Game.Models;
using Game.Providers;
using Game.Views;

namespace Game.Controllers
{
    public class RotatePartButtonController : ViewEnableController<RotateButtonView>, IRotatePartButtonController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public BuildingProcessModel BuildingProcessModel { get; private set; }
        [Inject] public SpritesProvider SpritesProvider { get; set; }

        private IDataViewController _data;

        public override void Enable()
        {
            _data = new RotateButtonData(
                SpritesProvider[SpriteID.RotatePartButtonActive],
                SpritesProvider[SpriteID.RotatePartButtonDefault]);

            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            BuildingProcessModel.OnBuildingStarted += OnBuildingEnabled;
            BuildingProcessModel.OnBuildingEnded += OnBuildingDisabled;

            UpdateViewVisible();
        }

        private void OnBuildingDisabled()
        {
            Hide();
        }

        private void OnBuildingEnabled()
        {
            if (IsCanShow)
            {
                Show();
            }
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

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();

        public override ViewConfigID ViewConfigID => ViewConfigID.RotatePartButton;
        public override IDataViewController Data => _data;

        public override bool IsCanShow => !PlayerHealthModel.IsDead && BuildingProcessModel.IsBuildingEnabled && CanRotate;
        
        // [Move ot other entity: RotatePart[Model/Controller] with events
        private bool CanRotate => BuildingProcessModel.SelectedPart.FreePlacement || BuildingProcessModel.SelectedPart.RotateOnSockets || !BuildingProcessModel.SelectedPart.RequireSockets;
    }
}
