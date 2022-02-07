using BehaviorDesigner.Runtime;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.AI;
using Game.Audio;
using Game.Models;
using Game.StateMachine.Parametrs;
using NaughtyAttributes;
using RoboRyanTron.SearchableEnum;
using System;
using System.Collections;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public class Attack : MonoBehaviour
    {
        public event Action<Target> OnKillTarget;

        #region Data
    #pragma warning disable 0649

        [SerializeField] private ObscuredFloat _damage;
        [SerializeField] BehaviorTree _behaviourTree;
        [SerializeField] private bool _useConfig;
        [ShowIf("_useConfig")] [SerializeField] private EnemyConfig _config;

        [SearchableEnum]
        [SerializeField] private AudioID[] _audioIDs;

    #pragma warning restore 0649
        #endregion

        public ObscuredFloat Damage => (_useConfig && _config != null ) ? (ObscuredFloat)(_config.AttackDamage * EnemiesModifierModel.DamageScaler) : _damage;
        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;
        private AudioSystem AudioSystem => AudioSystem.Instance;

        #region Dependencies

        private EnemiesModifierModel EnemiesModifierModel => ModelsSystem.Instance._enemiesModifierModel;
            
        #endregion

        private float _attackRange = 0;

        private void Start()
        {
            _attackRange = ((SharedFloat)_behaviourTree.GetVariable("AttackRange")).Value;
        }
        
        //from animation event
        void OnAnimationDamage()
        {
            SharedGameObject targetVaeiable = (SharedGameObject)_behaviourTree.GetVariable("Target");
            GameObject targetGO = (GameObject)targetVaeiable.GetValue();

            if (targetGO == null || !IsTargetInRange(targetGO)) return;

            Target target = targetGO.GetComponent<Target>();
            if (target == null) return;

            PlayAttackSound();

            HandleDamage();

            bool targetIsPlayer = target.ID == TargetID.Player;

            HandleTargetIsDead();
                
            void HandleDamage()
            {
                var damagable = target.GetComponentInParent<IDamageable>();
                if(damagable == null) damagable = target.GetComponentInChildren<IDamageable>();
                
                float damage = EnemiesAttackModificators.GetDamageForTarget(target.ID, Damage);

                if(damage == 0) return;

                if (damagable != null)
                    damagable.Damage(damage);
            }

            void HandleTargetIsDead()
            {
                IHealth healthInTarget = targetIsPlayer ? PlayerHealthModel : target.GetComponentInParent<IHealth>();

                if (healthInTarget == null) return;

                if (healthInTarget.IsDead)
                {
                    _behaviourTree.SendEvent("KillTarget");
                    OnKillTarget?.Invoke(target);
                }
            }
        }

        private void GetDamageForTarget(TargetID targetID)
        {

        }

        private bool IsTargetInRange(GameObject targetGO)
        {
            var distanceSqr = (transform.position - targetGO.transform.position).sqrMagnitude;
            bool answer = distanceSqr < _attackRange * _attackRange;
            return answer;
        }

        private void PlayAttackSound()
        {
            AudioID attackSound = GetRandomSound();
            if(attackSound != AudioID.None)
            {
                AudioSystem.PlayOnce(GetRandomSound(), transform.position);
            }
        }

        private AudioID GetRandomSound()
        { 
            if(_audioIDs.Length > 0)
                return _audioIDs[UnityEngine.Random.Range(0, _audioIDs.Length)];
            else
                return AudioID.None;
        }
    }
}