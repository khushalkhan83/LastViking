using Game.AI.BehaviorDesigner;
using Game.ThirdPerson.Weapon.Core.Interfaces;
using Game.ThirdPerson.Weapon.Settings.Implementation;
using UnityEngine;

namespace UltimateSurvival
{
    public class RaycastManager : PlayerBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Camera m_WorldCamera;
        [UnityEngine.Serialization.FormerlySerializedAs("m_RayLength")]
        [SerializeField] private float m_interactionRayLength = 1.5f;
        [SerializeField] private LayerMask m_LayerMask;
        [SerializeField] private OutlineManager m_outlineManager;
        [SerializeField] private FPManager FPManager;
        [SerializeField] private ThirdPersonMeleeDamager meleeDamager;

        [SerializeField] private float m_interactionRadius = 1f;

#pragma warning restore 0649
        #endregion

        private IWeaponInteractor _weaponInteractor;
        private Ray _ray;
        private RaycastHit _hitInfo;
        private RaycastData _raycastData = new RaycastData();

        Transform _customTarget;
        Vector3 _targetPoint;
        bool isUseTargetRayCollider = false;
        Collider targetCollier = null;

        private Collider[] _targetColliders = new Collider[10];
        private int _collidersCount = 0;

        private void OnEnable() 
        {
            _weaponInteractor = GetComponentInChildren<IWeaponInteractor>();
        }

        public void SetCurrentTarget(Transform t, Vector3 targetPoint)
        {
            // isUseTargetRayCollider = true;
            // _customTarget = t;
            // _targetPoint = targetPoint;
        }

        void UpdateOutline(IOutlineTarget targ)
        {
            
            m_outlineManager.SetOutline(targ);
        }

        float MeleeOrDefaultRayLength
        {
            get
            {
                if (_weaponInteractor != null && _weaponInteractor.WeaponEquiped)
                    return _weaponInteractor.HitZoneLength;
                else
                    return m_interactionRayLength;
            }
        }

        private void Update()
        {
            targetCollier = null;
            UpdateEnemiesTargets();
            if(targetCollier == null)
            {
                UpdateInteractableTargets();
            }

            var maxDistance = Mathf.Max(m_interactionRayLength,MeleeOrDefaultRayLength);

            bool hitSomething;
            if (targetCollier != null)
            {
                _ray.origin = GetRaycastStartPoint();
                _ray.direction = (targetCollier.bounds.center - _ray.origin).normalized;
                hitSomething = targetCollier.Raycast(_ray, out _hitInfo, maxDistance);

                // Play can be inside interactable collider, try raycast outside collider
                if(!hitSomething && _raycastData.ObjectIsInteractable)
                {
                    _ray.origin = _ray.origin - _ray.direction * m_interactionRayLength;
                    hitSomething = targetCollier.Raycast(_ray, out _hitInfo, m_interactionRayLength);
                }  
            }
            else
            {
                _ray.origin = GetRaycastStartPoint();
                _ray.direction = m_WorldCamera.transform.forward;
                hitSomething = Physics.Raycast(_ray, out _hitInfo, maxDistance, m_LayerMask);
            }

            Debug.DrawRay(_ray.origin, _ray.direction * maxDistance, Color.green);          

            if(hitSomething)
            {
                var distance = _raycastData.ObjectIsInteractable ? m_interactionRayLength: MeleeOrDefaultRayLength;
 
                IOutlineTarget targ = null;
                if (_hitInfo.transform != null)
                    targ = _hitInfo.collider.GetComponent<IOutlineTarget>();

                bool distanceOk = _hitInfo.distance <= distance;
                bool showOutlineCondition = distanceOk && targ != null;

                if(distanceOk)
                    SetDataHitSomething();
                else
                    SetDataHitNothing();

                if (showOutlineCondition)
                    UpdateOutline(targ);
                else
                    UpdateOutline(null);
            }
            else
            {
                SetDataHitNothing();
                UpdateOutline(null);
            }

            Player.CanUseWeapon = CanUseWeapon();
        }

        private void UpdateEnemiesTargets()
        {
            if(meleeDamager.CollidersCount > 0)
            {
                Collider closesTargetCollier = null;
                float closesDistance = float.MaxValue;
                for (int i = 0; i < meleeDamager.CollidersCount; i++)
                {
                    if(meleeDamager.TargetColliders[i].gameObject.GetComponent<EnemyDamageReciver>() != null)
                    {
                        float distnace = (transform.position - meleeDamager.TargetColliders[i].transform.position).sqrMagnitude;
                        if(distnace < closesDistance)
                        {
                            closesTargetCollier = meleeDamager.TargetColliders[i];
                            closesDistance = distnace;
                        }
                    }
                }

                if(closesTargetCollier != null)
                {
                    targetCollier = closesTargetCollier;
                }
            }
        }

        private void UpdateInteractableTargets()
        {
            Vector3 point1 = GetRaycastStartPoint() + m_WorldCamera.transform.forward * m_interactionRadius;
            Vector3 point2 = point1 + m_WorldCamera.transform.forward * (m_interactionRayLength - m_interactionRadius * 2);
            _collidersCount = Physics.OverlapCapsuleNonAlloc(point1, point2, m_interactionRadius, _targetColliders, m_LayerMask);

            if(_collidersCount > 0)
            {
                float closestAngle = float.MaxValue;
                for(int i = 0; i < _collidersCount; i++)
                {
                    if(_targetColliders[i].gameObject.GetComponent<EnemyDamageReciver>() == null)
                    {
                        Vector3 direction = _targetColliders[i].transform.position - point1;
                        float angle = Vector3.Angle(m_WorldCamera.transform.forward, direction);
                        if(angle < closestAngle)
                        {
                            closestAngle = angle;
                            targetCollier = _targetColliders[i];
                        }
                    }
                }
            }
        }

        private Vector3 GetRaycastStartPoint()
        {
            Vector3 playerVector = (Player.transform.position + Vector3.up) - m_WorldCamera.transform.position;
            return m_WorldCamera.transform.position + Vector3.Project(playerVector, m_WorldCamera.transform.forward);
        }

        private void SetDataHitSomething()
        {
            _raycastData.SetData(_hitInfo, _ray);
            Player.RaycastData.SetForce(_raycastData);
        }

        private void SetDataHitNothing()
        {
            Player.RaycastData.Set(null);
            UpdateOutline(null);
        }

        public bool CanUseWeapon()
        {
            if(Player.RaycastData.Value == null || Player.RaycastData.Value.GameObject == null)
            {
                return true;
            }
            else
            {
                return  !Player.RaycastData.Value.ObjectIsInteractable
                    && (Player.RaycastData.Value.GameObject.GetComponent<Game.Models.IDamageable>() != null
                    || Player.RaycastData.Value.GameObject.GetComponent<MineableObject>() != null);
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 point1 = GetRaycastStartPoint() + m_WorldCamera.transform.forward * m_interactionRadius;
            Vector3 point2 = point1 + m_WorldCamera.transform.forward * (m_interactionRayLength - m_interactionRadius * 2);
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(point1, m_interactionRadius);
            Gizmos.DrawWireSphere(point2, m_interactionRadius);
        }
        #endif
    }
}
