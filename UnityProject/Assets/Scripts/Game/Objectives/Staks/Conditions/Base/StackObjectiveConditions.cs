using System.Collections.Generic;
using UnityEngine;

namespace Game.Objectives.Stacks.Conditions.Base
{
    // class not abstact to serialize in inspector
    [CreateAssetMenu(fileName = "StackObjectiveConditions", menuName = "UnityProject/StackObjectiveConditions", order = 0)]
    public class StackObjectiveConditions : ScriptableObject
    {
        [SerializeField] private List<StackObjectiveCondition> conditions;

        public bool ConditionsOK()
        {
            foreach (var c in conditions)
            {
                bool ok = c.CheckCondition();
                if (!ok) return false;
            }
            return true;
        }
    }
}