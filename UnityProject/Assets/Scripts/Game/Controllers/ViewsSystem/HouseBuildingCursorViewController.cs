using Game.Views;
using Core.Controllers;
using UltimateSurvival;
using Core;
using Game.VillageBuilding;
using Extensions;
using Game.Models;
using static Game.Models.CraftViewModel;
using System.Collections.Generic;

namespace Game.Controllers
{
    public class HouseBuildingCursorViewController : ViewControllerBase<HouseBuildingCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CraftViewModel CraftViewModel { get; private set; }
        [Inject] public HouseBuildingViewModel HouseBuildingViewModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public VillageAttackModel VillageAttackModel { get; private set; }

        private Dictionary<HouseBuildingType,CategoryID> craftCategoryByBuildingType = new Dictionary<HouseBuildingType,CategoryID>()
        {
            {HouseBuildingType.Forge, CategoryID.Weapons},
            {HouseBuildingType.Alchemy, CategoryID.Medical},
            {HouseBuildingType.Tools, CategoryID.Tools},
            {HouseBuildingType.Workshop, CategoryID.Items},
        };

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnDownHandler();
            }
        }

        private void OnDownHandler()
        {
            HouseBuilding houseBuilding = PlayerEventHandler.RaycastData.Value?.GameObject.CheckNull()?.GetComponentInParent<HouseBuilding>();

            if(houseBuilding == null) return;

            if(VillageAttackModel.AttackModeActive) return;

            HouseBuildingType type = houseBuilding.Type;
            bool isCraftBuilding = craftCategoryByBuildingType.ContainsKey(type);
            bool isConstructed = houseBuilding.Level > 0;

            if(isCraftBuilding && isConstructed)
            {
                ViewsSystem.Show<CraftView>(ViewConfigID.Craft);
                CraftViewModel.SelectCategory(craftCategoryByBuildingType[type]);
            }
            else
            {
                HouseBuildingViewModel.SetTarget(houseBuilding);
                HouseBuildingViewModel.SetShow(true);
            }
        }
    }
}