using Core.StateMachine;
using Game.Audio;
using Game.StateMachine.Parametrs;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace Game.AI.Behaviours.Skeleton
{
    public class TargetDetect : BehaviourBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetDetection _targetDetection;
        [SerializeField] private TargetBase _player;
        [SerializeField] private TargetBase _main;
        [SerializeField] private Transform _root;
        [SerializeField] private ConditionBase _playerDetectCondition;
        [SerializeField] private ConditionBase _chillingCondition;
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private LayerMask _obstacleLayer;
        [SerializeField] private LayerMask _terrainLayer;
        [SerializeField] private float _lengthDifferencePaths;
        [SerializeField] private RunToPosition _runToPosition;
        [SerializeField] private float _zPointOffs = 0.01f;
        [SerializeField] private AudioID _audioID;
        [SerializeField] private OnceAudioThread _onceAudioThread;

#pragma warning restore 0649
        #endregion

        public AudioID AudioID => _audioID;
        public ConditionBase PlayerDetectCondition => _playerDetectCondition;
        public ConditionBase ChillingCondition => _chillingCondition;
        public float LengthDiffenercePaths => _lengthDifferencePaths;
        public LayerMask ObstacleLayer => _obstacleLayer;
        public LayerMask TerrainLayer => _terrainLayer;
        public TargetDetection TargetDetection => _targetDetection;
        public TargetBase Player => _player;
        public TargetBase Main => _main;
        public Transform Root => _root;
        public NavMeshAgent NavMeshAgent => _navMeshAgent;
        public RunToPosition RunToPosition => _runToPosition;
        public float ZPointOffs => _zPointOffs;
        public OnceAudioThread OnceAudioThread => _onceAudioThread;

        IEnumerator Finding;

        public override void Begin()
        {
            Finding = FindTarget();
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
            if (PlayerDetectCondition.IsTrue && !ChillingCondition.IsTrue)
            {
                TargetDetection.SetTarget(Player.Target);
                yield break;
            }

            yield return null;

            var position = Root.position;
            var positionMain = Main.Target.transform.position;
            var path = new NavMeshPath();
            var isHasPath = NavMesh.CalculatePath(position, positionMain, NavMesh.AllAreas, path);

            yield return null;

            var hits = Physics.RaycastAll(position, positionMain - position, (positionMain - position).magnitude, ObstacleLayer | TerrainLayer);

            yield return null;

            if (isHasPath && path.status == NavMeshPathStatus.PathComplete)
            {
                var pathLength = GetPathLength(path.corners, position);
                var forwardLength = (positionMain - position).magnitude;

                if (pathLength - forwardLength < LengthDiffenercePaths)
                {
                    TargetDetection.SetTarget(Main.Target);
                    OnceAudioThread.PlayOnce(AudioID);
                    yield break;
                }

                yield return null;

                if (TryGetFirstObstacle(hits, ObstacleLayer, TerrainLayer, out var obstacle, out var hitPoint))
                {
                    RunToPosition.TargetPosition = hitPoint;
                    TargetDetection.SetTarget(obstacle);
                    OnceAudioThread.PlayOnce(AudioID);
                    yield break;
                }

                yield return null;

                TargetDetection.SetTarget(Main.Target);
                OnceAudioThread.PlayOnce(AudioID);
                yield break;
            }
            else
            {
                if (TryGetFirstObstacle(hits, ObstacleLayer, TerrainLayer, out var obstacle, out var hitPoint))
                {
                    RunToPosition.TargetPosition = hitPoint;
                    TargetDetection.SetTarget(obstacle);
                    OnceAudioThread.PlayOnce(AudioID);
                    yield break;
                }
            }

            yield return null;
        }

        private bool TryGetFirstObstacle(RaycastHit[] hits, LayerMask obstacleLayer, LayerMask errorLayer, out Target obstacle, out Vector3 hitPoint)
        {
            obstacle = null;
            hitPoint = Vector3.zero;
            int bitLayer;

            foreach (var hit in hits)
            {
                bitLayer = GetBitLayer(hit.collider.gameObject.layer);
                if (bitLayer == errorLayer)
                {
                    return false;
                }

                if (obstacle == null && bitLayer == obstacleLayer)
                {
                    obstacle = hit.collider.gameObject.GetComponentInParent<Target>();

                    var colPos = hit.collider.transform.position;
                    var xDist = hit.point.x - colPos.x;
                    var heading = hit.point - Root.position;
                    var dir = Vector3.Dot(heading, Root.position) < 0 ? +1 : -1;
                    var colForward = hit.collider.transform.forward;

                    colForward.x = Mathf.Abs(colForward.x);
                    colForward.z = Mathf.Abs(colForward.z);

                    hitPoint = colPos + hit.collider.transform.right * xDist + dir * colForward * ZPointOffs;
                }
            }

            return obstacle != null;
        }

        private int GetBitLayer(int layer) => layer == 0 ? 0 : 1 << layer;

        private float GetPathLength(Vector3[] points, Vector3 currentPosition)
        {
            if (points.Length > 0)
            {
                var result = Vector3.Distance(currentPosition, points[0]);

                if (points.Length == 1)
                {
                    return result;
                }

                for (int i = 1; i < points.Length; ++i)
                {
                    result += Vector3.Distance(points[i - 1], points[i]);
                }

                return result;
            }

            return 0;
        }
    }
}
