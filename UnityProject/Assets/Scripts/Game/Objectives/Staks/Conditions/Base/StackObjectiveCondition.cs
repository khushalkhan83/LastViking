using UnityEngine;

namespace Game.Objectives.Stacks.Conditions.Base
{
    // class not abstact to serialize in inspector
    public class StackObjectiveCondition : ScriptableObject
    {
        public virtual bool CheckCondition() { return false; }
    }
} 