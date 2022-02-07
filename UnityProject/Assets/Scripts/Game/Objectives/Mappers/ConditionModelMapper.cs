using Core.Mapper;
using System;

namespace Game.Objectives
{
    public class ConditionModelMapper : MapperBase<ConditionID, Type>
    {
        private void Awake()
        {
            Map<CountConditionDataModel>(ConditionID.AnimalKill);
            Map<CountConditionDataModel>(ConditionID.BreakWorldObject);
            Map<CountConditionDataModel>(ConditionID.BuildItem);
            Map<CountConditionDataModel>(ConditionID.ConsumeItem);
            Map<CountConditionDataModel>(ConditionID.CookItem);
            Map<CountConditionDataModel>(ConditionID.CraftItem);
            Map<CountConditionDataModel>(ConditionID.EnemyKill);
            Map<CountConditionDataModel>(ConditionID.KeepFood);
            Map<CountConditionDataModel>(ConditionID.KeepHealth);
            Map<CountConditionDataModel>(ConditionID.KeepWater);
            Map<DoneConditionDataModel>(ConditionID.LoginLeaderboard);
            Map<CountConditionDataModel>(ConditionID.PickupItem);
            Map<CountConditionDataModel>(ConditionID.ReachPlaceLB);
            Map<CountConditionDataModel>(ConditionID.ReachScoreLB);
            Map<CountConditionDataModel>(ConditionID.RepairItem);
            Map<CountConditionDataModel>(ConditionID.ResourceCollect);
            Map<DoneConditionDataModel>(ConditionID.SubscribeDiscord);
            Map<DoneConditionDataModel>(ConditionID.UpgradeShelter);
            Map<CountConditionDataModel>(ConditionID.RepairAnyItem);
            Map<CountConditionDataModel>(ConditionID.PickupCoin);
            Map<CountConditionDataModel>(ConditionID.PickupBlueprint);
            Map<DoneConditionDataModel>(ConditionID.RaiseHealth);
            Map<DoneConditionDataModel>(ConditionID.RaiseFood);
            Map<DoneConditionDataModel>(ConditionID.RaiseWater);
            Map<CountConditionDataModel>(ConditionID.EnemyKillAny);
            Map<DoneConditionDataModel>(ConditionID.BuyShelter);
            Map<CountConditionDataModel>(ConditionID.BuildItemAny);
            Map<DoneConditionDataModel>(ConditionID.TombInteract);
        }

        public void Map<T>(ConditionID conditionID) where T : IConditionDataModel => Map(conditionID, typeof(T));
    }
}
