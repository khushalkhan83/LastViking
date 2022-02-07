using UnityEngine;

namespace Game.ThirdPerson.Weapon.Settings.Implementation
{
    public class RangedWeaponSettings : MonoBehaviour
    {
        public float damage {get; private set;}

        public void SetDamage(float damage)
        {
            this.damage = damage;
        }
    }
}