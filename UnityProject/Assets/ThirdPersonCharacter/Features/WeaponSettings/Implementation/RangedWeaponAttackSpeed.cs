using UnityEngine;

namespace Game.ThirdPerson.Weapon.Settings.Implementation
{
    public class RangedWeaponAttackSpeed : MonoBehaviour
    {
        private const string k_AttackSpeed = "RangeAttackSpeed";

        #region Data
        #pragma warning disable 0649
        [SerializeField] private Animator animator;
        
        #pragma warning restore 0649
        #endregion

        public void ApplySpeed(float speed)
        {
            animator.SetFloat(k_AttackSpeed, speed);
        }
    }
}