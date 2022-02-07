using Core.Mapper;

namespace Game.Objectives
{
    public class ConditionDataMapper : MapperBase<ConditionID, ConditionDataTypeID>
    {
        private void Awake()
        {
            Map(ConditionID.AnimalKill, ConditionDataTypeID.Int);
            Map(ConditionID.BreakWorldObject, ConditionDataTypeID.Int);
            Map(ConditionID.BuildItem, ConditionDataTypeID.Int);
            Map(ConditionID.ConsumeItem, ConditionDataTypeID.Int);
            Map(ConditionID.CookItem, ConditionDataTypeID.Int);
            Map(ConditionID.CraftItem, ConditionDataTypeID.Int);
            Map(ConditionID.EnemyKill, ConditionDataTypeID.Int);
            Map(ConditionID.KeepFood, ConditionDataTypeID.Int);
            Map(ConditionID.KeepHealth, ConditionDataTypeID.Int);
            Map(ConditionID.KeepWater, ConditionDataTypeID.Int);
            Map(ConditionID.LoginLeaderboard, ConditionDataTypeID.Bool);
            Map(ConditionID.PickupItem, ConditionDataTypeID.Int);
            Map(ConditionID.ReachPlaceLB, ConditionDataTypeID.Int);
            Map(ConditionID.ReachScoreLB, ConditionDataTypeID.Int);
            Map(ConditionID.RepairItem, ConditionDataTypeID.Int);
            Map(ConditionID.ResourceCollect, ConditionDataTypeID.Int);
            Map(ConditionID.SubscribeDiscord, ConditionDataTypeID.Bool);
            Map(ConditionID.UpgradeShelter, ConditionDataTypeID.Bool);
            Map(ConditionID.RepairAnyItem, ConditionDataTypeID.Int);
            Map(ConditionID.PickupCoin, ConditionDataTypeID.Int);
            Map(ConditionID.PickupBlueprint, ConditionDataTypeID.Int);
            Map(ConditionID.RaiseHealth, ConditionDataTypeID.Bool);
            Map(ConditionID.RaiseFood, ConditionDataTypeID.Bool);
            Map(ConditionID.RaiseWater, ConditionDataTypeID.Bool);
            Map(ConditionID.EnemyKillAny, ConditionDataTypeID.Int);
            Map(ConditionID.BuyShelter, ConditionDataTypeID.Bool);
            Map(ConditionID.BuildItemAny, ConditionDataTypeID.Int);
            Map(ConditionID.TombInteract, ConditionDataTypeID.Bool);
            Map(ConditionID.KillKraken, ConditionDataTypeID.Int);
            Map(ConditionID.BuildDock, ConditionDataTypeID.Bool);
            Map(ConditionID.BuildHouseFoundation, ConditionDataTypeID.Bool);
            Map(ConditionID.BuildHouseWalls, ConditionDataTypeID.Bool);
            Map(ConditionID.BuildHouseRoof, ConditionDataTypeID.Bool);
            Map(ConditionID.BuildHouseDoor, ConditionDataTypeID.Bool);
        }
    }
}
