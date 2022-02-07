using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.AI.Behaviours.Skeleton;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.States.Conditions.Skeleton
{
    public class PositionInRangeCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _range;
        [SerializeField] private RunToPosition _position;
        [SerializeField] private Movement _movement;

#if UNITY_EDITOR
        [SerializeField] private bool _isDrawGizmos;
        [SerializeField] private Color _gizmosColor;

#endif

#pragma warning restore 0649
        #endregion

        public ObscuredFloat Range => _range;
        public Movement Movement => _movement;
        public Vector3 TargetPosition => _position.TargetPosition;

        public override bool IsTrue => Vector3.Distance(TargetPosition, Movement.Position) <= Range;

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
