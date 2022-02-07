using System;
using Game.AI.BehaviorDesigner;
using Game.Audio;
using Game.Models;
using Game.ObjectPooling;
using MarchingBytes;
using NaughtyAttributes;
using UnityEngine;

namespace Game.AI.Components
{
    public class ExplosiveBarrel : MonoBehaviour, IDamageable, IResettable
    {
        [SerializeField] private GameObject _view;

        [Header("Effects on player")]
        [SerializeField] private float force;
        [SerializeField] private UltimateSurvival.GenericShake _genericShake;
        [SerializeField] private float _shakeScale;

        [SerializeField] private LayerMask _layerMask;
        [Header("Explosion Settings")]
        [SerializeField] private float _explosionRadius = 5;
        [SerializeField] private float _maxDamage = 100;
        [SerializeField] private bool _useEnemiesConfig;
        [ShowIf("_useEnemiesConfig")]
        [SerializeField] private EnemyConfig _enemyConfig;
        [SerializeField] private float _ownerDamageMultiply = 100;
        [SerializeField] private AnimationCurve _damageCoefByDistance;
        [SerializeField] private AudioID _audioID = AudioID.Explosion;

        [Header("Extra")]
        [SerializeField] private Collider owner;
        [SerializeField] private bool destroy = true;


        #region Properties
        private UltimateSurvival.PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private AudioSystem AudioSystem => AudioSystem.Instance;

        private float MaxDamage => (_useEnemiesConfig && _enemyConfig != null) ? _enemyConfig.AttackDamage : _maxDamage;

        #endregion

        private bool _exploded = false;

        void IDamageable.Damage(float value, GameObject from)
        {
            Explode();
        }
        
        public void ResetObject()
        {
            _exploded = false;
            _view.SetActive(true);
        }

        public void Explode()
        {
            if(_exploded) return;

            _exploded = true;
            _view.SetActive(false);
            AudioSystem.PlayOnce(_audioID,transform.position);
            CreateFX();
            AddImpactForPlayer();
            DoCameraShakeForPlayer();
            DamageDamagableInRadius(transform.position,_explosionRadius, owner == null ? null: owner.gameObject);
            DamageOwner();
            if(destroy)
            {
                Destroy(this); // destroy this object
            }
        }

        private void DamageOwner()
        {
            if(owner == null) return;
            
            var damagable = owner.GetComponent<IDamageable>();
            damagable?.Damage(MaxDamage * _ownerDamageMultiply,owner.gameObject);
        }

        private void DoCameraShakeForPlayer()
        {
            _genericShake.Shake(_shakeScale);
        }

        private void CreateFX()
        {
            EasyObjectPool.instance.GetObjectFromPool("BigExplosion", transform.position, Quaternion.identity, null);
        }

        private void AddImpactForPlayer()
        {
            Vector3 pointWithSameHightAsPlayer = transform.position;
            pointWithSameHightAsPlayer.y = PlayerEventHandler.transform.position.y;

            var heading = PlayerEventHandler.transform.position - pointWithSameHightAsPlayer;
            var distance = heading.magnitude;
            var direction = heading / distance; // This is now the normalized direction.

            var impactForce = GetImpactForce(distance,_explosionRadius);
            PlayerEventHandler.AddImpact(direction,impactForce);
        }

        private void DamageDamagableInRadius(Vector3 center, float radius, GameObject from)
        {
            Collider[] hitColliders = Physics.OverlapSphere(center, radius, _layerMask);

            foreach (var collider in hitColliders)
            {
                if (collider.gameObject == gameObject) continue; // ignore damaging barel iteslf
                if(collider == owner) continue; // ignore owner. 

                var damagable = collider.GetComponentInParent<IDamageable>();
                if (damagable == null) continue;
                float damage = GetDamage(radius, collider);

                var target = collider.GetComponentInParent<Target>();
                if(target != null)
                {
                    damage = EnemiesAttackModificators.GetDamageForTarget(target.ID, damage);
                }

                damagable?.Damage(damage, from);
            }
        }

        private float GetDamage(float radius, Collider collider)
        {
            var heading = collider.transform.position - transform.position;
            float distanceSqr = heading.sqrMagnitude;
            float distanceNormalized = distanceSqr / (radius * radius);

            var damageCoeficient = _damageCoefByDistance.Evaluate(distanceNormalized);
            var damage = MaxDamage * damageCoeficient;
            return damage;
        }

        private float GetImpactForce(float distance,float radius)
        {
            float distanceNormalized = distance / radius;

            var impactCoef = _damageCoefByDistance.Evaluate(distanceNormalized);
            var impact = force * impactCoef;
            return impact;
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, _explosionRadius);
        }
#endif
    }
}
