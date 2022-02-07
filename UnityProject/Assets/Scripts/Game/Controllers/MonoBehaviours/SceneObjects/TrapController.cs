using System.Collections;
using System.Collections.Generic;
using Game.AI.BehaviorDesigner;
using Game.Models;
using NaughtyAttributes;
using UltimateSurvival.Building;
using UnityEngine;

namespace Game.Controllers
{
    public class TrapController : MonoBehaviour
    {
        [SerializeField] GameObject readyView = default;
        [SerializeField] GameObject triggerdView = default;
        [SerializeField] BuildingHealthModel buildingHealthModel = default;
        [SerializeField] WorldObjectModel worldObjectModel = default;

        [Space(10)]
        [Header("Collision Settings")]
        [SerializeField] LayerMask layerMask = default;
        [SerializeField] ColliderType colliderType = default;
        [ShowIf("IsSphereCollider")]
        [SerializeField] float overlaRadius = 0.5f;
        [ShowIf("IsBoxCollider")]
        [SerializeField] private Vector3 overlapBoxSize = Vector3.one;
        [Space(10)]

        [SerializeField] float damageToEnemy = 50f;
        [SerializeField] float damageToItself = 35f;
        [SerializeField] float triggerDelay = 0.1f;
        [SerializeField] float resetTime = 10f;
        
        private bool initialized = false;
        private State state = State.Ready;
        private Collider[] colliders = new Collider[5];
        private float startTriggerTime;
        private float triggeredTime;

        private bool IsSphereCollider() => colliderType == ColliderType.Sphere;
        private bool IsBoxCollider() => colliderType == ColliderType.Box;

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
            if(initialized)
            {
                if(state == State.Ready)
                {
                    int count = Overlap();
                    for(int i = 0; i < count; i++ )
                    {
                        EnemyDamageReciver enemyDamageReciver = colliders[i].GetComponentInChildren<EnemyDamageReciver>();
                        if(enemyDamageReciver != null)
                        {
                            StartTrigger();
                            break;
                        }
                    }
                }
                else if(state == State.StartTrigger)
                {
                    if(Time.time - startTriggerTime > triggerDelay)
                    {
                        Triggered();
                    }
                }
                else if(state == State.Triggered)
                {
                    if(Time.time - triggeredTime > resetTime)
                    {
                        buildingHealthModel.AdjustHealth(-damageToItself);
                        if(!buildingHealthModel.IsDead)
                        {
                            ResetTrap();
                        }
                    }
                }
            }
        }

        private int Overlap()
        {
            if(IsSphereCollider())
            {
                return Physics.OverlapSphereNonAlloc(transform.position, overlaRadius, colliders, layerMask);
            }
            else
            {
                return Physics.OverlapBoxNonAlloc(transform.position, overlapBoxSize, colliders, transform.rotation, layerMask);
            }
        }

        private void StartTrigger()
        {
            state = State.StartTrigger;
            startTriggerTime = Time.time;
        }

        private void Triggered()
        {
            state = State.Triggered;
            triggeredTime = Time.time;
            SetReadyView(false);

            int count = Overlap();
            for(int i = 0; i < count; i++ )
            {
                EnemyDamageReciver enemyDamageReciver = colliders[i].GetComponentInChildren<EnemyDamageReciver>();
                if(enemyDamageReciver != null)
                {
                    ((IDamageable)enemyDamageReciver).Damage(damageToEnemy);
                }
            }
        }

        private void ResetTrap()
        {
            state = State.Ready;
            SetReadyView(true);
        }

        private void SetReadyView(bool isReady)
        {
            readyView.SetActive(isReady);
            triggerdView.SetActive(!isReady);
        }

        public enum State
        {
            Ready,
            StartTrigger,
            Triggered,
        }

        public enum ColliderType
        {
            Sphere,
            Box,
        }

        private void OnDrawGizmosSelected() 
        {
            #if UNITY_EDITOR
            Gizmos.color = Color.yellow;
            if(IsSphereCollider())
            {
                Gizmos.DrawWireSphere(transform.position, overlaRadius);
            }
            else
            {
                Matrix4x4 oldGizmosMatrix = Gizmos.matrix;
                Gizmos.matrix = transform.localToWorldMatrix;
                Gizmos.DrawWireCube(Vector3.zero, overlapBoxSize * 2);
                Gizmos.matrix = oldGizmosMatrix;
            }
            #endif
        }
    }
}
