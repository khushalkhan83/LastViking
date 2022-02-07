using Core;
using Game.Models;
using Game.Providers;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class PlaceItemController : ViewEnableController<PlaceButtonView>, IPlaceItemController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public SpritesProvider SpritesProvider { get; set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; set; }

        private IDataViewController _data;

        public override void Enable()
        {
            _data = new PlaceButtonData(
                SpritesProvider[SpriteID.PlaceButtonActive], 
                SpritesProvider[SpriteID.PlaceButtonDefault]);

            HotBarModel.OnChangeEquipItem += OnEquippedItemHandler;
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled += OnBuildingDisabled;
        }

        public override void Start()
        {
            UpdateViewVisible();
        }

        public override void Disable()
        {
            HotBarModel.OnChangeEquipItem -= OnEquippedItemHandler;
            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
            BuildingModeModel.BuildingEnabled -= OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled -= OnBuildingDisabled;

            Hide();
        }

        private void OnEquippedItemHandler() => UpdateViewVisible();
        private void OnChangePlayerMovement() => UpdateViewVisible();
        private void OnBuildingEnabled() => UpdateViewVisible();
        private void OnBuildingDisabled() => UpdateViewVisible();

        private bool IsItemPlaceable(SavableItem item) => item != null
            && item.Count > 0
            && item.HasProperty("Is Placeable");

        private bool IsPlayerGrounded => PlayerMovementModel.MovementID == PlayerMovementID.Ground;
        private bool IsBuildingMode => BuildingModeModel.BuildingActive;

        public override ViewConfigID ViewConfigID => ViewConfigID.PlaceButton;
        public override IDataViewController Data => _data;

        public override bool IsCanShow => 
                IsItemPlaceable(HotBarModel.EquipCell.Item) &&
                IsPlayerGrounded &&
                !IsBuildingMode;
    }
}
