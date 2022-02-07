using System;
using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using CodeStage.AntiCheat.ObscuredTypes;
using Game.AI;
using Game.Models;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public class SprintAttack : MonoBehaviour
    {
        public event Action<Target> OnKillTarget;

        #region Data
    #pragma warning disable 0649

        [SerializeField] ObscuredFloat _damage;
        [SerializeField] BehaviorTree _behaviorTree;
        [SerializeField] Collider _collider;
        
    #pragma warning restore 0649
        #endregion

        private SharedGameObject _targetVaeiable;

        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;


        private void OnEnable() 
        {
            _behaviorTree.RegisterEvent("StartSprintAttack", OnStartSprintAttack);
            _behaviorTree.RegisterEvent("StopSprintAttack", OnStopSprintAttack);
            _targetVaeiable = (SharedGameObject)_behaviorTree.GetVariable("Target");
        }

        private void OnDisable() 
        {
            _behaviorTree.UnregisterEvent("StartSprintAttack", OnStartSprintAttack);
            _behaviorTree.UnregisterEvent("StopSprintAttack", OnStopSprintAttack);
        }

        private void OnStartSprintAttack()
        {
            print("StartSprintAttack");
            _collider.enabled = true;
        }

        private void OnStopSprintAttack()
        {
            print("StopSprintAttack");
            _collider.enabled = false;
        }


        private void OnTriggerEnter(Collider other) 
        {
            Target target = other.GetComponent<Target>();

            if (target == null) 
                return;

            GameObject targetGO = (GameObject)_targetVaeiable.GetValue();

            if(targetGO == null || targetGO.GetComponent<Target>() != target)
                return;

            HandleDamage();

            bool targetIsPlayer = target.ID == TargetID.Player;

            HandleTargetIsDead();
                
            void HandleDamage()
            {
                var damagable = target.GetComponentInParent<IDamageable>();

                if (damagable != null)
                    damagable.Damage(_damage);
            }

            void HandleTargetIsDead()
            {
                IHealth healthInTarget = targetIsPlayer ? PlayerHealthModel : target.GetComponentInParent<IHealth>();

                if (healthInTarget == null) return;

                if (healthInTarget.IsDead)
                {
                    _behaviorTree.SendEvent("KillTarget");
                    OnKillTarget?.Invoke(target);
                }
            }

            _collider.enabled = false;
        }
    }
}
