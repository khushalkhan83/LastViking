using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UltimateSurvival.Building;
using Extensions;
using UnityEngine;
using EasyBuildSystem.Runtimes.Internal.Part;
using Game.VillageBuilding;

namespace Game.Controllers
{
    public class ActivateReplaceButtonViewController : ViewEnableController<ReplaceButtonView>, IController
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ReplaceBuildingModel ReplaceBuildingModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }


        public override ViewConfigID ViewConfigID => ViewConfigID.ReplaceButtonConfig;

        public override bool IsCanShow => 
            TutorialModel.IsComplete &&
            !ReplaceBuildingModel.BuildingReplaceActive 
            && (IsReplacebleBuildingInFront()
            && !IsPlaceableItemEquiped()
            && !BuildingModeModel.BuildingActive)
            ||
            (IsReplacebleConstructionInFront()
            && BuildingModeModel.BuildingActive);

        private GameObject raycastObject = null;

        public override void Enable()
        {
            PlayerEventHandler.RaycastData.OnChange += OnChangeRaycastDataHandler;
            BuildingModeModel.BuildingEnabled += UpdateViewVisible;
            BuildingModeModel.BuildingDisabled += UpdateViewVisible;
            HotBarModel.OnChangeEquipItem += UpdateViewVisible;
            
            raycastObject = PlayerEventHandler.RaycastData.Value?.GameObject;
            UpdateViewVisible();
        }

        public override void Start()
        {
            
        }

        public override void Disable()
        {
            PlayerEventHandler.RaycastData.OnChange -= OnChangeRaycastDataHandler;
            BuildingModeModel.BuildingEnabled -= UpdateViewVisible;
            BuildingModeModel.BuildingDisabled -= UpdateViewVisible;
            HotBarModel.OnChangeEquipItem -= UpdateViewVisible;
        }

        private void OnChangeRaycastDataHandler()
        {
            if(raycastObject != PlayerEventHandler.RaycastData.Value?.GameObject)
            {
                raycastObject = PlayerEventHandler.RaycastData.Value?.GameObject;
                UpdateViewVisible();
            }
        }

        private bool IsPlaceableItemEquiped()
        {  
            var item = HotBarModel.EquipCell.Item;
            return item != null && item.Count > 0 && item.HasProperty("Is Placeable");
        }

        private bool IsReplacebleBuildingInFront()
        {
            return raycastObject.CheckNull()?.GetComponentInParent<BuildingPiece>() != null;
        }

        private bool IsReplacebleConstructionInFront()
        {
            return raycastObject.CheckNull()?.GetComponentInParent<HouseBuilding>() != null;
        }
    }
}
