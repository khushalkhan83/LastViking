using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.Models;
using Game.StateMachine.Parametrs;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;
using IDamageable = Game.Models.IDamageable;

namespace Game.AI.Behaviours.Boar
{
    public class BoarDamageReciver : MonoBehaviour, IDamageable, UltimateSurvival.IHitReciver, UltimateSurvival.IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetDetection _targetDetection;
        [SerializeField] private StateProcessor _stateProcessor;
        [SerializeField] private State _stateHit;
        [SerializeField] private Interrupt[] _interrupts;
        [SerializeField] private State _stateDeath;
        [SerializeField] private ConditionBase _canGotHit;
        [SerializeField] private EffectBase _effectCantGotHit;
        [SerializeField] private EffectBase[] _effectsOnDeath;
        [SerializeField] private ObscuredInt _goldForKill;
        [SerializeField] private ObscuredInt _blueprintForKill;

        [SerializeField] List< Renderer> _renderers;
        [SerializeField] Vector3 _targetingShift;

#pragma warning restore 0649
        #endregion

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public TargetDetection TargetDetection => _targetDetection;
        public StateProcessor StateProcessor => _stateProcessor;
        public State StateHit => _stateHit;
        public State StateDeath => _stateDeath;
        public Interrupt[] Interrupts => _interrupts;
        public ConditionBase CanGotHit => _canGotHit;
        public EffectBase EffectCantGotHit => _effectCantGotHit;
        public EffectBase[] EffectsOnDeath => _effectsOnDeath;
        public int GoldForKill => _goldForKill;
        public int BlueprintForKill => _blueprintForKill;
        private float random;

        private AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;

        public event Action<IOutlineTarget> OnUpdateRendererList;


        public State State { get; private set; }

        private void Awake()
        {
            random = Random.Range(0, 100);
        }

        private void OnEnable()
        {
           UltimateSurvival.HitZoneManager.RegisterObjectGlobal(this);
        }
        void OnDisable()
        {
            UltimateSurvival.HitZoneManager.UnregisterObjectGlobal(this);
        }

        void IDamageable.Damage(float value, GameObject from)
        {
            var fromTarget = from.GetComponent<Target>();
            Health.AdjustHealth(-value);

            if (StateProcessor.State != StateHit)
            {
                State = StateProcessor.State;
            }

            if (Health.IsDead)
            {
                if (fromTarget.ID == TargetID.Player)
                {
                    StatisticsModel.Kill();
                    StatisticsModel.KillAnimal();
                }
                foreach (var effect in EffectsOnDeath)
                {
                    effect.Apply();
                }

                if (random <= 50f)
                {
                    CoinObjectsModel.SpawnAtPosition(GoldForKill, transform.position, transform.position + Vector3.up, 2f, AnimalID.Boar.ToString());
                }
                else
                {
                    BlueprintObjectsModel.SpawnAtPosition(BlueprintForKill, transform.position, transform.position + Vector3.up, 2f);
                }

                if (StateProcessor.State == StateHit)
                {
                    var deathInterrupt = new Interrupt();
                    deathInterrupt.SetState(StateDeath);
                    StateHit.SetNext(deathInterrupt);
                }
                else
                {
                    StateProcessor.TransitionTo(StateDeath);
                }
                AnimalsModel.TargetKillAnimal(fromTarget, AnimalID.Boar);
            }
            else
            {
                if (TargetDetection.Target != fromTarget)
                {
                    TargetDetection.SetTarget(fromTarget);
                    StateHit.SetNext(Interrupts);
                }
                else
                {
                    StateHit.Behaviour.OnEnd += OnEndHandler;
                }

                if (!State.IsLockInterruptTransition && CanGotHit.IsTrue && !Health.IsDead)
                {
                    StateProcessor.TransitionTo(StateHit);
                    EffectCantGotHit.Apply();
                }
            }
        }

        private void OnEndHandler(BehaviourBase behaviour)
        {
            behaviour.OnEnd -= OnEndHandler;
            StateHit.SetNext(Interrupts);
        }

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(_targetingShift);
        }

        public List< Renderer> GetRenderers()
        {
            return _renderers;
        }

        public bool IsUseWeaponRange()
        {
            return true;
        }
        public int GetColor()
        {
            return 0;
        }
    }
}
