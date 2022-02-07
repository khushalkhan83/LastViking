using System.Collections;
using BehaviorDesigner.Runtime;
using Game.Models;
using MarchingBytes;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace Game
{
    public class EnemySpawner : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private EnemyID enemyID = default;
        [SerializeField] private float respawTime = 45f;
        [SerializeField] private bool randomPoint = false;
        [ShowIf("randomPoint")]
        [SerializeField] private float randomPointRadius = 3f;

        #if UNITY_EDITOR
        [SerializeField] private Color ColorGizmos = Color.yellow;
        #endif

        private GameObject spawnedEnemy;
        
        #pragma warning restore 0649
        #endregion

        #region MonoBehaviour
        private void Start() 
        {
            Spawn();
        }
        #endregion

        private void Spawn()
        {
            try
            {
                spawnedEnemy = EasyObjectPool.instance.GetObjectFromPool(enemyID.ToString(), GetSpawnPosition(), Quaternion.identity, transform);
                Initable enemyInstance = spawnedEnemy.GetComponentInChildren<Initable>();
                enemyInstance.Init();
                
                var health = spawnedEnemy.GetComponentInChildren<EnemyHealthModel>();
                health.OnDeath += OnDeath;
            }
            catch (System.Exception)
            {
                Debug.LogException(new System.Exception("Handle Enemy Spawner error. Try Respawn"));
                StartCoroutine(Respawn(respawTime));
            }
        }

        private Vector3 GetSpawnPosition()
        {
            if(randomPoint)
            {
                var position = transform.position + Random.insideUnitSphere.normalized * Random.Range(0.0f, randomPointRadius);
                if (NavMesh.SamplePosition(position, out var hit, randomPointRadius * 1.1f, NavMesh.AllAreas))
                {
                    return hit.position;
                }
            }

            return transform.position;
        }

        private void OnDeath()
        {
            var health = spawnedEnemy.GetComponentInChildren<EnemyHealthModel>();
            health.OnDeath -= OnDeath;
            spawnedEnemy.transform.GetComponentInChildren<BehaviorTree>().RegisterEvent<object>("OnDisable", ReturnObjectToPool);
            spawnedEnemy = null;
            StartCoroutine(Respawn(respawTime));
        }

        private void ReturnObjectToPool(object obj)
        {
            GameObject enemyGO = obj as GameObject;
            // Reset view position
            enemyGO.transform.GetChild(0).localPosition = Vector3.zero;
            enemyGO.transform.GetComponentInChildren<BehaviorTree>().UnregisterEvent<object>("OnDisable", ReturnObjectToPool);
            
            PoolObject poolObject = enemyGO.GetComponentInParent<PoolObject>();
            if(poolObject != null)
            {
                EasyObjectPool.instance.ReturnObjectToPool(poolObject.gameObject);
            }
        }

        private IEnumerator Respawn(float time)
        {
            yield return new WaitForSeconds(time);
            Spawn();
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = ColorGizmos;
            if(randomPoint)
                Gizmos.DrawSphere(transform.position, randomPointRadius);
            else
                Gizmos.DrawSphere(transform.position, 1f);
        }
        #endif

    }
}
