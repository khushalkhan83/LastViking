using Game.Models;
using Game.Objectives.Stacks.Conditions.Base;
using Game.Providers;
using UnityEngine;

namespace Game.Objectives.Stacks.Conditions
{
    [CreateAssetMenu(fileName = "StackObjectiveConditionShelterNotMaxLevel", menuName = "UnityProject/StackObjectiveConditionShelterNotMaxLevel", order = 0)]
    public class StackObjectiveConditionShelterNotMaxLevel : StackObjectiveCondition
    {
        [SerializeField] private int shipMaxLevel = 3;

        private ShelterModelsProvider ShelterModelsProvider => ModelsSystem.Instance._shelterModelsProvider;
        private ShelterModelID ShipModelID => ShelterModelID.Ship;
        private ShelterModel ShipShelter => ShelterModelsProvider[ShipModelID];

        public override bool CheckCondition()
        {
            bool shipIsMaxLevel = ShipShelter.Level >= shipMaxLevel - 1;
            return !shipIsMaxLevel;
        }
    }
}