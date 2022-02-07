using UnityEngine;

namespace Game.Objectives.Data.Conditions.Static
{
    public class AnimalKillConditionData : ConditionBaseData, IProgress<int>
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private AnimalID _animalID;
        [SerializeField] private int _killCount;

#pragma warning restore 0649
        #endregion

        public AnimalID AnimalID => _animalID;
        public int Value => _killCount;

        public override ConditionID ConditionID => ConditionID.AnimalKill;
    }
}
