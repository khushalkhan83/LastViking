using Game.Models;
using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class BuyShelterConditionData : ConditionBaseData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ShelterModelID _shelterModelID;

#pragma warning restore 0649
        #endregion

        public ShelterModelID ShelterModelID => _shelterModelID;

        public override ConditionID ConditionID => ConditionID.BuyShelter;
    }
}
