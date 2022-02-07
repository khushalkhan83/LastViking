using Game.Audio;
using Game.Models;
using UnityEngine;
using cakeslice;

namespace UltimateSurvival
{
    public class FPMeleeSplash : FPMelee
    {

        #region Data
#pragma warning disable 0649
        [SerializeField] float _radius;
        [SerializeField] private LayerMask _layerMask;
#pragma warning restore 0649
        #endregion

        private Collider[] _targetColliders = new Collider[10];
        private int _collidersCount = 0;

        private Transform WorldCameraTransform => ModelsSystem.Instance._fPManager.WorldCamera.transform;
        private HintsModel HintsModel => ModelsSystem.Instance._hintsModel;

        protected virtual void Update()
        {
            DisableOldOutline();
            UpdateTargets();
            EnableNewOutline();
        }

        private void UpdateTargets()
        {
            Vector3 point1 = transform.position + WorldCameraTransform.forward * _radius;
            Vector3 point2 = point1 + WorldCameraTransform.forward * (MaxReach - _radius * 2);
            _collidersCount = Physics.OverlapCapsuleNonAlloc(point1, point2, _radius, _targetColliders, _layerMask);
        }

        private void DisableOldOutline()
        {
            for (int i = 0; i < _collidersCount; i++)
            {
                GameObject targetObject = null;
                if (_targetColliders[i] != null)
                {
                    targetObject = _targetColliders[i].gameObject;
                }

                if (targetObject == null)
                {
                    continue;
                }

                IOutlineTarget target = targetObject.GetComponent<IOutlineTarget>();

                if (target != null)
                {
                    foreach (Renderer rend in target.GetRenderers())
                    {
                        if (rend != null)
                        {
                            MonoBehaviour outl = rend.GetComponent<Outline>();
                            if (outl == null)
                                continue;

                            outl.enabled = false;
                        }
                    }
                }
            }
        }

        private void EnableNewOutline() {
            for (int i = 0; i < _collidersCount; i++) {
                IOutlineTarget target = _targetColliders[i].gameObject.GetComponent<IOutlineTarget>();
                if (target != null)
                {
                    foreach (Renderer rend in target.GetRenderers())
                    {
                        if (rend != null && rend.isVisible)
                        {
                            MonoBehaviour outl = rend.GetComponent<Outline>();
                            if (outl == null)
                                outl = rend.gameObject.AddComponent<Outline>();

                            (outl as Outline).color = target.GetColor();
                            outl.enabled = true;
                        }
                    }
                }
            }
        }

        public override bool TryAttackOnce(Camera camera)
        {
            if (Time.time < m_NextUseTime)
            {
                return false;
            }

            MeleeAttack.Send(_collidersCount > 0);

            StartCoroutine(AttackSound());
            // Send the regular attack message.
            Attack.Send();

            m_NextUseTime = Time.time + TimeBetweenAttacks;

            return true;
        }

        protected override void On_Hit()
        {
            OnHitSound();

            bool enemyHited = false;
            for (int i = 0; i < _collidersCount; i++)
            {
                var damageableBuilding = _targetColliders[i].GetComponent<DamageReceiver>();
                if (damageableBuilding)
                {
                    if (IsCanHitBuilding)
                    {
                        OnHitBuilding(damageableBuilding);
                    }
                }
                else
                {
                    var damageable = _targetColliders[i].GetComponent<Game.Models.IDamageable>();
                    if (damageable != null && IsCanHitDamageble)
                    {
                        Helpers.DamagableHelper.ShowDamage(damageable, DamagePerHit);

                        damageable.Damage(DamagePerHit, Player.gameObject);
                        enemyHited = true;
                    }
                }

                HintsModel.TryShowMinableToolHint(_targetColliders[i].gameObject);
            }
            if (enemyHited)
            {
                OnDecreaseDurability();
            }
           
            Hit.Send();
        }

        protected void OnHitBuilding(DamageReceiver damageableBuilding)
        {
            var damageable = damageableBuilding.GetComponent<Game.Models.IDamageable>();
            var healthBuilding = damageableBuilding.GetComponentInParent<BuildingHealthModel>();

            if (damageableBuilding.ID == PlayerTargetItem.Buildings)
            {
                if (damageable != null)
                {
                    var damageValue = healthBuilding.HealthMax / 3 + 1;

                    Helpers.DamagableHelper.ShowDamage(damageable, damageValue);

                    PlayerEventHandler.PlayerAttackBuilding = true;
                    damageable.Damage(damageValue, Player.gameObject);
                    PlayerEventHandler.PlayerAttackBuilding = false;
                }
            }
        }

        protected override void OnHitSound()
        {
            for (int i = 0; i < _collidersCount; i++)
            {
                var targetCollider = _targetColliders[i];
                var audioIdentifier = targetCollider.GetComponent<AudioIdentifier>();

                if (audioIdentifier)
                {
                    AudioSystem.PlayOnce(audioIdentifier.AudioID[Random.Range(0, audioIdentifier.AudioID.Length)], targetCollider.transform.position);
                }
            }
        }

        protected override void On_Woosh()
        {
            if (_collidersCount == 0)
            {
                Miss.Send();
            }
        }

        protected override void OnDisable() {
            base.OnDisable();
            DisableOldOutline();
        }
    }
  
}
