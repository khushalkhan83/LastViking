using Core.Mapper;
using Game.Objectives.Conditions.Controllers;
using System;

namespace Game.Objectives
{
    public class ConditionControllersMapper : MapperBase<ConditionID, Type>
    {
        private void Awake()
        {
            Map<AnimalKillConditionController>(ConditionID.AnimalKill);
            Map<EnemyKillConditionController>(ConditionID.EnemyKill);
            Map<ItemPickUpConditionController>(ConditionID.PickupItem);
            Map<ItemConsumeConditionController>(ConditionID.ConsumeItem);
            Map<CookItemConditionController>(ConditionID.CookItem);
            Map<CraftItemConditionController>(ConditionID.CraftItem);
            Map<ResourceCollectConditionController>(ConditionID.ResourceCollect);
            Map<UpgradeShelterConditionController>(ConditionID.UpgradeShelter);
            Map<BuildItemConditionController>(ConditionID.BuildItem);
            Map<BreakWorldObjectConditionController>(ConditionID.BreakWorldObject);
            Map<RepairItemConditionController>(ConditionID.RepairItem);
            Map<RepairAnyItemConditionController>(ConditionID.RepairAnyItem);
            Map<PickupCoinConditionController>(ConditionID.PickupCoin);
            Map<PickupBlueprintConditionController>(ConditionID.PickupBlueprint);
            Map<RaiseHealthConditionController>(ConditionID.RaiseHealth);
            Map<RaiseFoodConditionController>(ConditionID.RaiseFood);
            Map<RaiseWaterConditionController>(ConditionID.RaiseWater);
            Map<EnemyKillAnyConditionController>(ConditionID.EnemyKillAny);
            Map<BuyShelterConditionController>(ConditionID.BuyShelter);
            Map<BuildItemAnyConditionController>(ConditionID.BuildItemAny);
            Map<TombInteractConditionController>(ConditionID.TombInteract);
            Map<KillKrakenConditionController>(ConditionID.KillKraken);
            Map<BuildDockConditionController>(ConditionID.BuildDock);
            Map<BuildHouseFoundationConditionController>(ConditionID.BuildHouseFoundation);
            Map<BuildHouseDoorConditionController>(ConditionID.BuildHouseDoor);
            Map<BuildHouseWallsConditionController>(ConditionID.BuildHouseWalls);
            Map<BuildHouseRoofConditionController>(ConditionID.BuildHouseRoof);
        }

        public void Map<T>(ConditionID conditionID) where T : IConditionController, new() => Map(conditionID, typeof(T));
    }
}
