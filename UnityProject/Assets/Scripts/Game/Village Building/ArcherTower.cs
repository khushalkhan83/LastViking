using System.Collections;
using System.Collections.Generic;
using Game.AI.BehaviorDesigner;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.AI;

namespace Game.VillageBuilding
{
    public class ArcherTower : MonoBehaviour
    {
        [SerializeField] private float range = 15f;
        [SerializeField] private float blindRange = 5f;
        [SerializeField] private float cooldown = 5f;
        [SerializeField] private Vector3 targetOffset = default;
        [SerializeField] private Transform shootPoint = default;
        [SerializeField] private Transform groundPoint = default;
        [SerializeField] private ShaftedProjectile arrowPrefab = default;
        [SerializeField] private LayerMask enemiesLayerMask = default;
        [SerializeField] private bool smartAim = true;

        private Collider [] colliders = new Collider[10];
        private float lastShootTime = 0;
        private float sqrBlindRange;

        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;

        private void OnEnable()
        {
            sqrBlindRange = blindRange * blindRange;
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void OnDisable() 
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if(Time.time - lastShootTime >= cooldown)
            { 
                int collidersCount = Physics.OverlapSphereNonAlloc(groundPoint.position, range, colliders, enemiesLayerMask);
                if(collidersCount > 0)
                {
                    var enemyColider = GetClosestEnemy(collidersCount);
                    if(enemyColider != null)
                    {
                        Shoot(enemyColider);
                    }
                }
            }
        }

        private Collider GetClosestEnemy(int collidersCount)
        {
            Collider closest = null;
            float closestDistance = float.MaxValue;
            for(int i = 0; i < collidersCount; i++)
            {
                var col = colliders[i];

                if(col.GetComponent<EnemyDamageReciver>() == null) 
                    continue;

                float sqrDistance = (col.transform.position - groundPoint.position).sqrMagnitude;
                if(sqrDistance < closestDistance && sqrDistance > sqrBlindRange)
                {
                    closest = col;
                    closestDistance = sqrDistance;
                }
            }
            return closest;
        }

        private void Shoot(Collider enemyCollider)
        {
            Vector3 targetPoint = GetTargetPoint(enemyCollider);
            var rotation = Quaternion.LookRotation(targetPoint - shootPoint.position);
            ShaftedProjectile projectile = Instantiate(arrowPrefab, shootPoint.position, rotation);
            projectile.Launch(null, 0);
            lastShootTime = Time.time;
        }

        private Vector3 GetTargetPoint(Collider enemyCollider)
        {
            Vector3 enemyPosition = enemyCollider.transform.position + targetOffset;
            if(smartAim)
            {
                var nawMeshAgent = enemyCollider.GetComponentInParent<NavMeshAgent>();
                if(nawMeshAgent != null)
                {
                    Vector3 enemySpeed = nawMeshAgent.velocity;
                    return FirstOrderIntercept(shootPoint.position, Vector3.zero, arrowPrefab.LaunchSpeed, enemyPosition, enemySpeed);
                }
            }
            return enemyPosition;
        }


        public static Vector3 FirstOrderIntercept(Vector3 shooterPosition, Vector3 shooterVelocity, float shotSpeed, Vector3 targetPosition, Vector3 targetVelocity)  
        {
            Vector3 targetRelativePosition = targetPosition - shooterPosition;
            Vector3 targetRelativeVelocity = targetVelocity - shooterVelocity;
            float t = FirstOrderInterceptTime(shotSpeed, targetRelativePosition, targetRelativeVelocity);
            return targetPosition + t*(targetRelativeVelocity);
        }

        public static float FirstOrderInterceptTime (float shotSpeed, Vector3 targetRelativePosition, Vector3 targetRelativeVelocity) 
        {
            float velocitySquared = targetRelativeVelocity.sqrMagnitude;
            if(velocitySquared < 0.001f)
                return 0f;
        
            float a = velocitySquared - shotSpeed*shotSpeed;
        
            //handle similar velocities
            if (Mathf.Abs(a) < 0.001f)
            {
                float t = -targetRelativePosition.sqrMagnitude/
                (
                    2f*Vector3.Dot
                    (
                        targetRelativeVelocity,
                        targetRelativePosition
                    )
                );
                return Mathf.Max(t, 0f); //don't shoot back in time
            }
        
            float b = 2f*Vector3.Dot(targetRelativeVelocity, targetRelativePosition);
            float c = targetRelativePosition.sqrMagnitude;
            float determinant = b*b - 4f*a*c;
        
            if (determinant > 0f) { //determinant > 0; two intercept paths (most common)
                float	t1 = (-b + Mathf.Sqrt(determinant))/(2f*a),
                        t2 = (-b - Mathf.Sqrt(determinant))/(2f*a);
                if (t1 > 0f) {
                    if (t2 > 0f)
                        return Mathf.Min(t1, t2); //both are positive
                    else
                        return t1; //only t1 is positive
                } else
                    return Mathf.Max(t2, 0f); //don't shoot back in time
            } else if (determinant < 0f) //determinant < 0; no intercept path
                return 0f;
            else //determinant = 0; one intercept path, pretty much never happens
                return Mathf.Max(-b/(2f*a), 0f); //don't shoot back in time
        }

        public void OnDrawGizmos()
        {
#if UNITY_EDITOR
            if (groundPoint == null) {
                return;
            }
            var oldColor = UnityEditor.Handles.color;
            UnityEditor.Handles.color = Color.red;
            UnityEditor.Handles.DrawWireDisc(groundPoint.position, groundPoint.up, blindRange);
            UnityEditor.Handles.color = Color.yellow;
            UnityEditor.Handles.DrawWireDisc(groundPoint.position, groundPoint.up, range);
            UnityEditor.Handles.color = oldColor;
#endif
        }
    }
}
