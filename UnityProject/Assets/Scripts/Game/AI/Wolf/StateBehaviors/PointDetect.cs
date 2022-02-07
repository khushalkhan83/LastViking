using Core.StateMachine;
using Core.States.Parametrs;
using Game.Audio;
using Game.StateMachine.Behaviours;
using Game.StateMachine.Parametrs;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.AI.Behaviours.Wolf
{
    public class PointDetect : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _rangeDetect;
        [SerializeField] private Movement _movement;
        [SerializeField] private PositionParametr _targetPosition;
        [SerializeField] private TransformProvider _targetPost;
        [SerializeField] private AudioID[] _audioIDs;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public AudioID[] AudioIDs => _audioIDs;
        public float RangeDetect => _rangeDetect;
        public Movement Movement => _movement;
        public PositionParametr PositionParametr => _targetPosition;
        public TransformProvider TargetPost => _targetPost;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

        private AudioID RandomSound => AudioIDs[UnityEngine.Random.Range(0, AudioIDs.Length)];

        IEnumerator Finding;

        public override void Begin()
        {
            Finding = FindTarget();
            OnceAudioThread.PlayOnce(RandomSound);
        }

        public override void ForceEnd()
        {
            End();
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

            NavMesh.Raycast(position, TargetPost.Target.transform.position + direction * RangeDetect, out var navMeshHit, NavMesh.AllAreas);

            PositionParametr.Position = navMeshHit.position;

            yield return null;
        }
    }
}
