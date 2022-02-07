using Core.Mapper;
using Game.Objectives.Actions.Controllers;
using Game.Objectives.Controllers;
using System;

namespace Game.Objectives
{
    public class ActionControllersMapper : MapperBase<ActionID, Type>
    {
        private void Awake()
        {
            Map<RewardGoldActionController>(ActionID.RewardGold);
            Map<RewardItemActionController>(ActionID.RewardItem);
            Map<RewardBluePrintsActionController>(ActionID.BluePrintsReward);
            Map<ChoiseItemActionDataController>(ActionID.ChoiseItem);
            Map<ChangeControllersStateActionDataController>(ActionID.ChangeControllersState);
            Map<OutLineMineableStoneActionDataController>(ActionID.OutLineMineableStone);
            Map<ActivateTokenActionController>(ActionID.ActivateToken);
            Map<ApplyModificatorActionController>(ActionID.ApplyModificator);
            Map<ShowGhostDockActionDataController>(ActionID.ShowGhostDock);
            Map<ShowGhostFoundationActionController>(ActionID.ShowGhostFoundation);
            Map<ShowGhostWallsActionController>(ActionID.ShowGhostWalls);
            Map<ShowGhostRoofActionController>(ActionID.ShowGhostRoof);
            Map<ShowGhostDoorActionController>(ActionID.ShowGhostDoor);
            Map<DisableBuildingCategoriesActionDataController>(ActionID.DisableBuildingCategories);
        }

        public void Map<T>(ActionID actionID) where T : IActionController, new() => Map(actionID, typeof(T));
    }
}
