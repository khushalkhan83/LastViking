using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using UnityEngine;
using UnityEngine.AI;

namespace Game.StateMachine.Behaviours
{
    public class Movement : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private ObscuredFloat _speedWalk;
        [SerializeField] private ObscuredFloat _speedRun;
        [SerializeField] private ObscuredFloat _speedSmooth;

#pragma warning restore 0649
        #endregion

        public ObscuredFloat SpeedWalk => _speedWalk;
        public ObscuredFloat SpeedRun => _speedRun;
        public ObscuredFloat SpeedSmooth => _speedSmooth;

        public NavMeshAgent NavMeshAgent => _navMeshAgent;

        public Vector3 Destination => NavMeshAgent.destination;
        public Vector3 Position => NavMeshAgent.transform.position;
        public Vector3 Forward => NavMeshAgent.transform.forward;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        public float SpeedSmoothNormalized { get; private set; }

        private void OnEnable()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            SpeedSmoothNormalized = Mathf.MoveTowards(SpeedSmoothNormalized, NavMeshAgent.desiredVelocity.magnitude / SpeedRun, Time.deltaTime * SpeedSmooth);
        }

        public void WalkTo(Vector3 destination, float stopDistance = 0) => MoveTo(destination, SpeedWalk, stopDistance);

        public void RunTo(Vector3 destination, float stopDistance = 0) => MoveTo(destination, SpeedRun, stopDistance);

        public void __SlowRunTo(Vector3 destination, float stopDistance = 0) => MoveTo(destination, SpeedRun / 2, stopDistance);

        /* Try NavMesh acceleration */

        public void Stop()
        {
            if (NavMeshAgent.isActiveAndEnabled)
            {
                NavMeshAgent.velocity = Vector3.zero;
                NavMeshAgent.isStopped = true;
            }
        }

        private void MoveTo(Vector3 destination, float speed, float stopDistance = 0)
        {
            if (!NavMeshAgent.enabled)
            {
                NavMeshAgent.enabled = true;
            }
            NavMeshAgent.isStopped = false;
            NavMeshAgent.stoppingDistance = stopDistance;
            NavMeshAgent.speed = speed;
            NavMeshAgent.SetDestination(destination);
        }
    }
}
