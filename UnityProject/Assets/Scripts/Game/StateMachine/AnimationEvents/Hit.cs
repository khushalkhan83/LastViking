using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.AI;
using Game.Audio;
using Game.Models;
using Game.StateMachine.Parametrs;
using System;
using UnityEngine;
using NaughtyAttributes;
using Game.AI.Behaviours.Kraken;

namespace Game.StateMachine.Events
{
    public class Hit : MonoBehaviour, IKrakenConfigurable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private bool useDamageValueProvider;
        [ShowIf("useDamageValueProvider")] [SerializeField] private ValueProviderScriptable _damageValueProviderScriptable;
        [SerializeField] private ObscuredFloat _damage;
        [SerializeField] private ConditionBase _attackCondition;
        [SerializeField] private TargetDetection _targetDetection;
        [SerializeField] private AudioID[] _audioIDs;
        [SerializeField] private GameObject _mouth;

#pragma warning restore 0649
        #endregion

        public float Damage => useDamageValueProvider && _damageValueProviderScriptable != null ? (_damageValueProviderScriptable as IDamageValueProvider).DamageValue :(float)_damage;
        public AudioID[] AudioIDs => _audioIDs;
        public ConditionBase AttackCondition => _attackCondition;

        private AudioSystem AudioSystem => AudioSystem.Instance;
        private PlayerBleedingDamagerModel PlayerBleedingDamagerModel => ModelsSystem.Instance._playerBleedingDamagerModel;
        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;

        private TargetDetection TargetDetection => _targetDetection;

        public event Action<Target> OnTargetDamaged;
        public event Action<Target> OnKillTarget;

        private AudioID RandomSound => AudioIDs[UnityEngine.Random.Range(0, AudioIDs.Length)];

        //from animation event
        void OnAnimationDamage()
        {
            if (!AttackCondition.IsTrue) return;

            AudioSystem.PlayOnce(RandomSound, _mouth.transform.position);

            if (TargetDetection == null) return;

            var target = TargetDetection.Target;
            if (target == null) return;

            HandleDamage();

            bool targetIsPlayer = target.ID == TargetID.Player;

            HandleTargetIsDead();

            #region Help functions
                
            void HandleDamage()
            {
                var damagable = target.GetComponentInParent<IDamageable>();

                if (damagable == null) return;

                damagable.Damage(Damage);
                OnTargetDamaged?.Invoke(target);
            }

            void HandleTargetIsDead()
            {
                IHealth healthInTarget = targetIsPlayer ? PlayerHealthModel : target.GetComponentInParent<IHealth>();

                if (healthInTarget == null) return;

                if (healthInTarget.IsDead)
                    OnKillTarget?.Invoke(target);
            }
            #endregion
        }

        public void DoDamage() => OnAnimationDamage();

        #region IKrakenConfigurable
        public void Configurate(KrakenConfig config)
        {
            if(!useDamageValueProvider)
            {
                Debug.LogError("Can`t configure component. Check settings");
                return;
            }
            _damageValueProviderScriptable = config;
        }
            
        #endregion
    }
}
