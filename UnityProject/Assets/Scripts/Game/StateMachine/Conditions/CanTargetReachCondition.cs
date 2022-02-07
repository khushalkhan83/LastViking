using Core.StateMachine;
using Game.StateMachine.Parametrs;
using UnityEngine;
using UnityEngine.AI;

namespace Game.StateMachine.Conditions
{
    public class CanTargetReachCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private TargetBase _target;
        [SerializeField] private float _distance;

#pragma warning restore 0649
        #endregion

        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public TargetBase Target => _target;
        public float Distance => _distance;

        public override bool IsTrue => Target.Target != null && CanReach();

        public bool CanReach()
        {
            float dist = Vector3.Distance(NavMeshAgent.destination, Target.Target.transform.position);
            return !NavMeshAgent.hasPath || dist <= Distance;
        }
    }
}
