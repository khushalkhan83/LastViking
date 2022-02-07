using System.Collections;
using System.Collections.Generic;
using Game.Models;
using Game.ThirdPerson.Weapon.Core.Interfaces;
using UltimateSurvival;
using UnityEngine;

namespace Game.ThirdPerson
{
    public class AttackStarter : MonoBehaviour
    {
        [SerializeField] private float cooldown = 1f;

        private Animator animator;
        private IWeaponInteractor weapon;
        private float nextUseTime;

        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        private void Awake() 
        {
            animator = GetComponent<Animator>();
            weapon = GetComponentInChildren<IWeaponInteractor>();
        }

        private void Update() 
        {
            if(PlayerInput.Instance.AttackTap && CanAttack())
            {
                nextUseTime = Time.time + cooldown;
                animator.SetTrigger("MeleeAttack");
            }
        }

        private bool CanAttack() => Time.time >= nextUseTime && PlayerEventHandler.CanUseWeapon && !weapon.RangedWeaponEquiped;

        public void SetCooldown(float cooldown)
        {
            this.cooldown = cooldown;
        }
    }
}
