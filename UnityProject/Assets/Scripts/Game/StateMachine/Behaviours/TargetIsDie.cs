using Core.StateMachine;
using Game.AI;
using Game.Models;
using Game.StateMachine.Parametrs;
using UnityEngine;

namespace Game.StateMachine.Behaviours
{
    public class TargetIsDie : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetDetection _targetDetection;
        [SerializeField] private State _stateNext;
        [SerializeField] private EffectBase _dieTargeteffct;
        [SerializeField] private StateProcessor _stateProcessor;

#pragma warning restore 0649
        #endregion

        private TargetDetection TargetDetection => _targetDetection;
        public State StateNext => _stateNext;
        private EffectBase DieTargetEffect => _dieTargeteffct;
        private StateProcessor StateProcessor => _stateProcessor;

        private IHealth Health { get; set; }
        private PlayerDeathModel PlayerDeathModel {get => ModelsSystem.Instance._playerDeathModel;}

        private void OnEnable()
        {
            TargetDetection.OnChangeTarget += OnChangeTarget;
            SubscribeOnTargetDeath();
        }

        private void OnDisable()
        {
            TargetDetection.OnChangeTarget -= OnChangeTarget;
            UnsubscribeOnTargetDeath();
        }

        private void OnChangeTarget()
        {
            UnsubscribeOnTargetDeath();
            SubscribeOnTargetDeath();
        }

        private void SubscribeOnTargetDeath()
        {
            if (TryGetTargetHealth(TargetDetection.Target, out var health))
            {
                Health = health;
                Health.OnDeath += OnDeathTarget;
            }
        }

        private void UnsubscribeOnTargetDeath()
        {
            if (Health != null)
            {
                Health.OnDeath -= OnDeathTarget;
            }
        }

        private bool TryGetTargetHealth(Target target, out IHealth health)
        {
            if (target == null)
            {
                health = null;
                return false;
            }

            if (target.ID == TargetID.Player)
            {
                health = ModelsSystem.Instance._playerHealthModel;
                return true;
            }

            health = null;
            return false;
        }

        private void OnDeathTarget()
        {
            TargetDetection.OnChangeTarget -= OnChangeTarget;

            if(TargetDetection.Target == null || TargetDetection.Target.ID != TargetID.Player) return;

            PlayerDeathModel.OnRevival += OnRevivalTarget;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalTarget;
        }

        private void OnRevivalTarget()
        {
            PlayerDeathModel.OnRevival -= OnRevivalTarget;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalTarget;

            TargetDetection.OnChangeTarget += OnChangeTarget;

            DieTargetEffect.Apply();
        }

        private void FreezeAI() => StateProcessor.gameObject.SetActive(false);
        private void UnfreezeAI() => StateProcessor.gameObject.SetActive(true);
    }
}
