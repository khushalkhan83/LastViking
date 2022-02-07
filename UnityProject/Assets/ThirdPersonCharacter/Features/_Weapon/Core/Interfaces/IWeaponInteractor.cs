using Game.Models;

namespace Game.ThirdPerson.Weapon.Core.Interfaces
{
    public interface IWeaponInteractor
    {
        void Equip(PlayerWeaponID weaponID);
        bool WeaponEquiped {get;}
        bool RangedWeaponEquiped {get;}
        bool MeleeWeaponEquiped {get;}
        float Damage {get;}
        float Cooldown {get;}
        float HitZoneRadius {get;}
        float HitZoneLength {get;}
    }
}