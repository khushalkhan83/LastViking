using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.AI.Behaviours.Skeleton;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.AI.States.Conditions.Skeleton
{
    public class PositionInFieldOfViewCondition : ConditionBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _root;
        [SerializeField] private RunToPosition _position;
        [SerializeField] private ObscuredFloat _fieldOfView;

#if UNITY_EDITOR
        [Header("UNITY_EDITOR")]
        [SerializeField] private bool _isDrawGizmos;
        [SerializeField] private Color _gizmosColor;
        [SerializeField] private PositionInRangeCondition _positionInRangeCondition;

#endif

#pragma warning restore 0649
        #endregion

        public ObscuredFloat FieldOfView => _fieldOfView;
        public Transform Root => _root;
        public Vector3 TargetPosition => _position.TargetPosition;

        Vector3 RootForward => GetVectorWithoutY(Root.transform.forward);
        Vector3 RootPosition => GetVectorWithoutY(Root.transform.position);
        Vector3 targetPosition => GetVectorWithoutY(TargetPosition);

        private Vector3 GetVectorWithoutY(Vector3 vec)
        {
            vec.y = 0;
            return vec;
        }

        public override bool IsTrue => Vector3.Angle(RootForward, targetPosition - RootPosition) < FieldOfView / 2;

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            if (_isDrawGizmos)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawRay(Root.position, Quaternion.Euler(0, FieldOfView / 2, 0) * Root.transform.forward * (_positionInRangeCondition?.Range ?? 1));
                Gizmos.DrawRay(Root.position, Quaternion.Euler(0, -FieldOfView / 2, 0) * Root.transform.forward * (_positionInRangeCondition?.Range ?? 1));
            }
        }
#endif
    }
}
