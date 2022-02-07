using Game.Models;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.AI;

namespace Encounters
{
    public interface ISpawnPointProvider
    {
        bool TryGetValidSpawnPoint(out Vector3 answer);
    }

    public class SpawnPointProvider : MonoBehaviour, ISpawnPointProvider
    {
        private const int k_randomPointAttemptsCount = 30;

        #region Data
#pragma warning disable 0649
        [SerializeField] private float _radius;
        [SerializeField] private bool _pathValidation = true;
        [SerializeField] private GameObject _pathValidationTraget = default;

        [Header("Test")]
        [SerializeField] private EnemyID _enemyID;

#if UNITY_EDITOR
        [SerializeField] private Color ColorGizmos;
#endif

#pragma warning restore 0649
        #endregion

        private Encounters.EnemyFactory factory = new Encounters.EnemyFactory();

        // private GameObject instance;

        private float Radius => _radius;
        private bool PathValidation => _pathValidation;


        [Button]
        private void TestSpawn() => CreatePrefab();

        private void CreatePrefab()
        {
            if (RandomPoint(this.transform.position, Radius, out var spawnPoint))
            {
                // var instance = GameObject.Instantiate(prefab,GetPositionRandom(),Quaternion.identity,this.transform);
                var enemy = factory.GetEnemy(spawnPoint, this.transform, new Encounters.EnemyConfig(_enemyID),true,null);
                enemy.Init();
            }
            else
            {
                Debug.LogError("Can`t get position");
            }
        }

        public bool TryGetValidSpawnPoint(out Vector3 answer)
        {
            var result = RandomPoint(this.transform.position, Radius, out var spawnPoint);
            answer = spawnPoint;

            if(result == false)
            {
                Debug.LogException(new System.Exception("Can't find spawn point"));
            }
            
            return result;
        }


        // TODO: remove code dublicate (same as range spawner)
        private bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            for (int i = 0; i < k_randomPointAttemptsCount; i++)
            {
                Vector3 randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, Radius * 1.2f, NavMesh.AllAreas))
                {
                    if (PathValidation && _pathValidationTraget != null)
                    {
                        var path = new NavMeshPath();
                        bool success = NavMesh.CalculatePath(hit.position, _pathValidationTraget.transform.position, NavMesh.AllAreas, path);
                        if (!success || path.status != NavMeshPathStatus.PathComplete)
                        {
                            Debug.Log("Partial path");
                            continue;
                        }
                    }

                    result = hit.position;
                    return true;
                }
            }
            result = Vector3.zero;
            return false;
        }

        #region MonoBehaviour
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = ColorGizmos;
            Gizmos.DrawWireSphere(transform.position, Radius);
            Gizmos.DrawSphere(transform.position, Radius);
        }
#endif
        #endregion
    }
}