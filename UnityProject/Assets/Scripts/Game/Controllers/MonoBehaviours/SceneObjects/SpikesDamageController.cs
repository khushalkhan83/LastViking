using System.Collections;
using System.Collections.Generic;
using Game.AI.BehaviorDesigner;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class SpikesDamageController : MonoBehaviour
    {
        [SerializeField] WorldObjectModel worldObjectModel = default;
        [SerializeField] private float damage = 5f;
        [SerializeField] private float damageInterval = 5f;
        [SerializeField] private LayerMask layerMask = default;
        [SerializeField] private Vector3 overlapBoxSize = Vector3.one;

        private bool initialized = false;
        private float lastDamageTime = 0;
        private Collider[] colliders = new Collider[5];


        private void OnEnable() 
        {
            worldObjectModel.OnDataInitialize += OnDataInitialize;
        }

        private void OnDisable() 
        {
            worldObjectModel.OnDataInitialize -= OnDataInitialize;
        }

        private void OnDataInitialize() => initialized = true;

        private void Update() 
        {
            if(initialized && Time.time - lastDamageTime > damageInterval)
            {
                DoDamage();
            }
        }

        private void DoDamage()
        {
            lastDamageTime = Time.time;
            int count = Physics.OverlapBoxNonAlloc(transform.position, overlapBoxSize, colliders, transform.rotation, layerMask);
            for(int i = 0; i < count; i++ )
            {
                EnemyDamageReciver enemyDamageReciver = colliders[i].GetComponent<EnemyDamageReciver>();
                if(enemyDamageReciver != null)
                {
                    ((IDamageable)enemyDamageReciver).Damage(damage);
                }
            }
        }

        private void OnDrawGizmosSelected() 
        {
            #if UNITY_EDITOR
            Gizmos.color = Color.yellow;
            Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, overlapBoxSize * 2);
            Gizmos.matrix = oldGizmosMatrix;
            #endif
        }
    }
}
