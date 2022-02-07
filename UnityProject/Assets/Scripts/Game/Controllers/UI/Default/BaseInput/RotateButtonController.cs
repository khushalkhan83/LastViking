using Core;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using Game.Providers;

namespace Game.Controllers
{
    public class RotateButtonController : ViewEnableController<RotateButtonView>, IRotateButtonController
    {
        [Inject] public RotateBuildablePreviewModel RotateBuildablePreviewModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public SpritesProvider SpritesProvider { get; set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; set; }
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; set; }
        [Inject] public ItemsDB ItemsDB { get; set; }

        private IDataViewController _data;

        public override void Enable()
        {
            _data = new RotateButtonData(
                SpritesProvider[SpriteID.RotateButtonActive],
                SpritesProvider[SpriteID.RotateButtonDefault]);

            HotBarModel.OnChangeEquipCell += OnChangeEquipCellHandler;
            HotBarModel.OnChangeEquipItem += OnChangeEquipCellHandler;
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
            BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled += OnBuildingDisabled;
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged += OnReplaceActiveChanged;
        }

        public override void Start()
        {
            UpdateViewVisible();
        }

        public override void Disable()
        {
            HotBarModel.OnChangeEquipCell -= OnChangeEquipCellHandler;
            HotBarModel.OnChangeEquipItem -= OnChangeEquipCellHandler;
            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
            BuildingModeModel.BuildingEnabled -= OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled -= OnBuildingDisabled;
            ReplaceBuildingModel.OnReplaceBuildingActiveChanged += OnReplaceActiveChanged;

            Hide();
        }

        private void OnChangeEquipCellHandler() => UpdateViewVisible();
        private void OnChangeEquipItemHandler() => UpdateViewVisible();
        private void OnChangePlayerMovement() => UpdateViewVisible();
        private void OnBuildingEnabled() => UpdateViewVisible();
        private void OnBuildingDisabled() => UpdateViewVisible();
        private void OnReplaceActiveChanged() => UpdateViewVisible();

        private bool IsItemRotatable
        {
            get
            {
                if(HotBarModel.EquipCell.IsHasItem && HotBarModel.EquipCell.Item.HasProperty("CanPlaceableRotation"))
                {
                    return true;
                }

                if(ReplaceBuildingModel.BuildingReplaceActive && ReplaceBuildingModel.Building != null)
                {
                    var itemData = ItemsDB.GetItem(ReplaceBuildingModel.Building.ItemID);
                    return itemData.IsHasProperty("CanPlaceableRotation");
                }

                return false;
            }
        }
        private bool IsPlayerGrounded => PlayerMovementModel.MovementID == PlayerMovementID.Ground;
        private bool IsBuildingMode => BuildingModeModel.BuildingActive;

        public override ViewConfigID ViewConfigID => ViewConfigID.RotateButton;
        public override IDataViewController Data => _data;

        public override bool IsCanShow => 
                IsItemRotatable &&
                IsPlayerGrounded &&
                !IsBuildingMode;

        protected override void OnShow()
        {
            RotateBuildablePreviewModel.Reset();
            PlayerEventHandler.RotateObject.Try(RotateBuildablePreviewModel.RotationAngleCurrent);
        }
    }
}
