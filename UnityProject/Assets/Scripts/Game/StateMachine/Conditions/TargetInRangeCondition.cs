using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Conditions
{
    public class TargetInRangeCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _range;
        [SerializeField] private TargetBase _target;
        [SerializeField] private Movement _movement;

#if UNITY_EDITOR
        [SerializeField] private bool _isDrawGizmos;
        [SerializeField] private Color _gizmosColor;

#endif

#pragma warning restore 0649
        #endregion

        public ObscuredFloat Range => _range;
        public Movement Movement => _movement;
        public TargetBase Target => _target;

        public override bool IsTrue => Target.Target && Vector3.Distance(Target.Target.transform.position, Movement.Position) <= Range;

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (_isDrawGizmos)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawSphere(Movement.Position, Range);
                Gizmos.DrawWireSphere(Movement.Position, Range);
            }
        } 
#endif
    }
}
