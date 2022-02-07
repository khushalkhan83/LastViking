using UnityEngine;

namespace Core.StateMachine.Operations.Conditions
{
    public class AgregatorCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ConditionBase[] _conditions;

#pragma warning restore 0649
        #endregion

        public ConditionBase[] Conditions => _conditions;

        public override bool IsTrue
        {
            get
            {
                foreach (var condition in Conditions)
                {
                    if (condition.IsTrue)
                    {
                        continue;
                    }

                    return false;
                }

                return true;
            }
        }
    }
}
