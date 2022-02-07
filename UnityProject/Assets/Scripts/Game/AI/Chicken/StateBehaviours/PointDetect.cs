using Core.StateMachine;
using Core.States.Parametrs;
using Game.StateMachine.Behaviours;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.AI.Behaviours.Chicken
{
    public class PointDetect : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _rangeDetect;
        [SerializeField] private Movement _movement;
        [SerializeField] private PositionParametr _targetPosition;

#pragma warning restore 0649
        #endregion

        public float RangeDetect => _rangeDetect;
        public Movement Movement => _movement;
        public PositionParametr PositionParametr => _targetPosition;

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
            yield return null;

            var position = Movement.Position;
            var angle = Random.Range(0, 360f);
            var direction = Quaternion.Euler(0, angle, 0) * Movement.Forward;

            NavMesh.Raycast(position, position + direction * RangeDetect, out var navMeshHit, NavMesh.AllAreas);

            PositionParametr.Position = navMeshHit.position;

            yield return null;
        }
    }
}
