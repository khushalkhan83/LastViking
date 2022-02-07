using Game.Models;
using Game.Objectives.Stacks.Conditions.Base;
using UnityEngine;

namespace Game.Objectives.Stacks.Conditions
{
    [CreateAssetMenu(fileName = "StackObjectiveConditionShelterExist", menuName = "UnityProject/StackObjectiveConditionShelterExist", order = 0)]
    public class StackObjectiveConditionShelterExist : StackObjectiveCondition
    {
        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
        private ShelterModelID ShipModelID => ShelterModelID.Ship;

        [SerializeField] private bool conditionOkIfShelterExist;

        public override bool CheckCondition()
        {
            bool shipShelterExist = SheltersModel.ShelterActive == ShipModelID;
            bool answer = conditionOkIfShelterExist ? shipShelterExist : !shipShelterExist;
            return answer;
        }
    }
}