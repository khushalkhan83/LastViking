using System.Linq;
using Game.ThirdPerson.Weapon.Settings.Interfaces;
using UltimateSurvival;
using UnityEngine;

namespace Game.ThirdPerson.Weapon.Settings.Implementation
{
    public class WeaponSettings : MonoBehaviour, IWeaponSettings
    {
        private const float k_defaultSpeed = 1;
        private const float k_defaultDamage = 1;
        private const float k_defaultHitZoneRadius = 1;
        private const float k_defaultHitZoneLength = 3.5f;
        private const float k_defaultCooldown = 1f;

        [SerializeField] private ThirdPersonMeleeDamager damager;
        [SerializeField] private RangedWeaponSettings rangedSetings;

        [SerializeField] private MeleeWeaponAttackSpeed animationSpeed;
        [SerializeField] private RangedWeaponAttackSpeed rangedAnimationSpeed;
        [SerializeField] private AttackStarter attackStarter;


        public float Damage {get; private set;}
        public bool RangedWeaponEquiped {get; private set;}
        public bool MeleeWeaponEquiped {get; private set;}
        public float Cooldwon {get; private set;}
        public float HitZoneRadius {get; private set;}
        public float HitZoneLength {get; private set;}

        #region MonoBehaviour
        private void Awake()
        {
            SetSettings(null);
        }
        #endregion

        public void SetSettings(SetSettingsRequest settings)
        {
            SetDefaultSettings();

            if(settings == null) return;

            Damage = settings.Damage;
            Cooldwon = settings.Cooldwon;
            HitZoneRadius = settings.HitZoneRadius;
            HitZoneLength = settings.HitZoneLength;
            if(settings.IsRange)
            {
                rangedAnimationSpeed.ApplySpeed(settings.Speed);
                rangedSetings.SetDamage(settings.Damage);
                damager.SetHitZoneRadius(settings.HitZoneRadius);
                damager.SetHitZoneLength(settings.HitZoneLength);
                attackStarter.SetCooldown(settings.Cooldwon);
                RangedWeaponEquiped = true;
            }
            else
            {
                damager.SetDamage(settings.Damage);
                damager.SetHitZoneRadius(settings.HitZoneRadius);
                damager.SetHitZoneLength(settings.HitZoneLength);
                damager.SetExtractionSettings(settings.Extraction.ToArray());
                animationSpeed.ApplySpeed(settings.Speed);
                attackStarter.SetCooldown(settings.Cooldwon);
                MeleeWeaponEquiped = true;
            }
        }

        private void SetDefaultSettings()
        {
            Damage = 0;
            RangedWeaponEquiped = false;
            MeleeWeaponEquiped = false;

            rangedAnimationSpeed.ApplySpeed(k_defaultSpeed);
            rangedSetings.SetDamage(k_defaultDamage);

            damager.SetDamage(k_defaultDamage);
            damager.SetHitZoneRadius(k_defaultHitZoneRadius);
            damager.SetHitZoneLength(k_defaultHitZoneLength);
            damager.SetExtractionSettings(new ExtractionSetting[0]);
            animationSpeed.ApplySpeed(k_defaultSpeed);
            attackStarter.SetCooldown(k_defaultCooldown);
        }
    }
}