using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Core.States.Parametrs;
using Game.Audio;
using Game.StateMachine.Behaviours;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace Game.AI.Behaviours.Bear
{
    public class PointDetectSoundBear : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _rangeDetect;
        [SerializeField] private Movement _movement;
        [SerializeField] private PositionParametr _targetPosition;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public AudioID AudioID => _audioID;
        public ObscuredFloat RangeDetect => _rangeDetect;
        public Movement Movement => _movement;
        public PositionParametr PositionParametr => _targetPosition;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

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
            var direction = Movement.Forward;

            float angle;
            NavMeshHit navMeshHit;

            yield return null;

            angle = Random.Range(0, 360f);
            direction = Quaternion.Euler(0, angle, 0) * direction;
            NavMesh.Raycast(position, position + direction * RangeDetect, out navMeshHit, NavMesh.AllAreas);

            PositionParametr.Position = navMeshHit.position;

            OnceAudioThread.PlayOnce(AudioID);

            yield return null;
        }
    }
}
