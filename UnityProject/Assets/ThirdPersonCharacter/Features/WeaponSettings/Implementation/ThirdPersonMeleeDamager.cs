using System.Collections;
using System.Linq;
using Game.Audio;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using IDamageable = Game.Models.IDamageable;

namespace Game.ThirdPerson.Weapon.Settings.Implementation
{
    public class ThirdPersonMeleeDamager : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private float _damage = 10;
        [SerializeField] private float _hitZoneRadius = 1f;
        [SerializeField] private float _hitZoneLength = 2f;
        [SerializeField] private Vector3 _shiftPosition = Vector3.up;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private AudioID _audioBroken;
        [SerializeField] private ExtractionSetting[] _extractionSettings;

        #pragma warning restore 0649
        #endregion
        private Collider[] _targetColliders = new Collider[10];
        private int _collidersCount = 0;

        public  Collider[] TargetColliders => _targetColliders;
        public  int CollidersCount => _collidersCount;
        public float HitZoneRadius => _hitZoneRadius;
        public float HitZoneLength => _hitZoneLength;

        #region Dependencies
        private PlayerEventHandler Player => ModelsSystem.Instance._playerEventHandler;
        private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;
        protected PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private AudioSystem AudioSystem => AudioSystem.Instance;
        private HintsModel HintsModel => ModelsSystem.Instance._hintsModel;
            
        #endregion

        public void SetDamage(float damage) => _damage = damage;
        public void SetHitZoneRadius(float hitZoneRadius) => _hitZoneRadius = hitZoneRadius;
        public void SetHitZoneLength(float hitZoneLength) => _hitZoneLength = hitZoneLength;
        public void SetExtractionSettings(ExtractionSetting[] settings) => _extractionSettings = settings;

        #region MonoBehaviour

        private void Update()
        {
            UpdateTargets();
        }


        #endregion

        private void UpdateTargets()
        {
            Vector3 point1 = transform.position + _shiftPosition + transform.forward * _hitZoneRadius;
            Vector3 point2 = point1 + transform.forward * (_hitZoneLength - _hitZoneRadius * 2);
            _collidersCount = Physics.OverlapCapsuleNonAlloc(point1, point2, _hitZoneRadius, _targetColliders, _layerMask);
        }

        public void DoDamageInZone()
        {
            bool weaponUsed = false;
            for (int i = 0; i < _collidersCount; i++)
            {
                var damageableBuilding = _targetColliders[i].GetComponent<DamageReceiver>();
                if (damageableBuilding)
                {
                    continue;
                }

                var damageable = _targetColliders[i].GetComponentInParent<Game.Models.IDamageable>();
                if (damageable != null)
                {
                    Helpers.DamagableHelper.ShowDamage(damageable, _damage);
                    damageable.Damage(_damage, Player.gameObject);
                    weaponUsed = true;

                    HintsModel.TryShowMinableToolHint(_targetColliders[i].gameObject);
                }
            }
            AudioSystem.PlayOnce(AudioID.Whoosh02);

            weaponUsed = weaponUsed || TryUseTool();

            if(weaponUsed)
            {
                DecreaseDurability();
            }
        }


        private bool TryUseTool()
        {
            var raycastData = Player.RaycastData.Value;
            if (raycastData == null
                || raycastData.GameObject == null
                || HotBarModel.EquipCell == null
                || HotBarModel.EquipCell.Item == null
                || HotBarModel.EquipCell.Item.ItemData == null)
                return false;

            var mineable = raycastData.GameObject.GetComponent<MineableObject>();

            HintsModel.CheckHintIsNeeded(mineable);

            if (mineable)
            {
                mineable.OnToolHit(raycastData.CameraRay, raycastData.HitInfo, _extractionSettings);

                var tool = _extractionSettings.FirstOrDefault(x => x.ToolID == mineable.RequiredToolPurpose);
                if (tool != null)
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual void DecreaseDurability()
        {
            if(HotBarModel.EquipCell != null && HotBarModel.EquipCell.IsHasItem)
            {
                var equipItem = HotBarModel.EquipCell.Item;
                if (equipItem != null && equipItem.TryGetProperty("Durability", out var durability))
                {
                    if(EditorGameSettings.Instance.BreakWeaponImmediately)
                    {
                        durability.Float.Current = 0;
                    }
                    else
                    {
                        durability.Float.Current--;
                    }
                    
                    if (durability.Float.Current <= 0)
                    {
                        AudioSystem.PlayOnce(_audioBroken, transform.position);
                    }
                    equipItem.SetProperty(durability);
                }
            }
        }

        #if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Vector3 point1 = transform.position + _shiftPosition + transform.forward * _hitZoneRadius;
            Vector3 point2 = point1 + transform.forward * (_hitZoneLength - _hitZoneRadius * 2);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(point1, _hitZoneRadius);
            Gizmos.DrawWireSphere(point2, _hitZoneRadius);
        }
        #endif
    }
}