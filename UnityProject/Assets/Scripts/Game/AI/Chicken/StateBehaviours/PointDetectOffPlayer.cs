using Core.StateMachine;
using Core.States.Parametrs;
using Game.StateMachine.Behaviours;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;
using Game.StateMachine.Parametrs;

namespace Game.AI.Behaviours.Chicken
{
    public class PointDetectOffPlayer : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _rangeDetect;
        [SerializeField] private float _offAngleRange = 75;
        [SerializeField] private Movement _movement;
        [SerializeField] private PositionParametr _targetPosition;
        [SerializeField] private TargetBase _target;

#pragma warning restore 0649
        #endregion

        public float RangeDetect => _rangeDetect;
        public float OffAngleRange => _offAngleRange;
        public Movement Movement => _movement;
        public PositionParametr PositionParametr => _targetPosition;
        public TargetBase Target => _target;

        IEnumerator Finding;

        public override void Begin()
        {
            Finding = FindTarget();
        }

        public override void ForceEnd()
        {
            StopCoroutine(Finding);
        }

        public override void Refresh()
        {
            if (!Finding.MoveNext())
            {
                End();
            }
        }

        private IEnumerator FindTarget()
        {
            var position = Movement.Position;
            var direction = -(Target.Target.transform.position - Movement.transform.position).normalized;
            
            float angle;
            NavMeshHit navMeshHit;

            yield return null;

            angle = Random.Range(0, OffAngleRange);
            direction = Quaternion.Euler(0, angle, 0) * direction;
            NavMesh.Raycast(position, position + direction * RangeDetect, out navMeshHit, NavMesh.AllAreas);

            PositionParametr.Position = navMeshHit.position;

            yield return null;
        }
    }
}
