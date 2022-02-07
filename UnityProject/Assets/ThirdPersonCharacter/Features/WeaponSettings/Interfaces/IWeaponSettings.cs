using UltimateSurvival;

namespace Game.ThirdPerson.Weapon.Settings.Interfaces
{
    public interface IWeaponSettings
    {
        void SetSettings(SetSettingsRequest request);

        bool RangedWeaponEquiped {get;}
        bool MeleeWeaponEquiped {get;}
        float Damage {get;}
        float Cooldwon {get;}
        float HitZoneRadius {get;}
        float HitZoneLength {get;}
    }

    public class SetSettingsRequest
    {
        public readonly bool IsRange;
        public readonly float Damage;
        public readonly float Speed;
        public readonly float Cooldwon;
        public readonly float HitZoneRadius;
        public readonly float HitZoneLength;

        public readonly ExtractionSetting[] Extraction;

        public SetSettingsRequest(bool isRange, float damage, float speed, float cooldown, float hitZoneRadius, float hitZoneLength,  ExtractionSetting[] extraction)
        {
            IsRange = isRange;
            Damage = damage;
            Speed = speed;
            Cooldwon = cooldown;
            Extraction = extraction;
            HitZoneRadius = hitZoneRadius;
            HitZoneLength = hitZoneLength;
        }
    }
}