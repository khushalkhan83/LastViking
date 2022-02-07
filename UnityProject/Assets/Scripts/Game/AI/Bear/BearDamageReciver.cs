using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Core.States.Parametrs;
using Game.Models;
using Game.StateMachine.Parametrs;
using UnityEngine;
using UltimateSurvival;
using Random = UnityEngine.Random;
using IDamageable = Game.Models.IDamageable;
using System;
using System.Collections.Generic;

namespace Game.AI.Behaviours.Bear
{
    public class BearDamageReciver : MonoBehaviour, IDamageable, IHitReciver, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetDetection _targetDetection;
        [SerializeField] private StateProcessor _stateProcessor;
        [SerializeField] private BoolParametr _is2LegsParametr;
        [SerializeField] private State _stateHit2Legs;
        [SerializeField] private State _stateHit4Legs;
        [SerializeField] private Interrupt[] _interrupts;
        [SerializeField] private ConditionBase _canGotHit;
        [SerializeField] private EffectBase _effectCantGotHit;
        [SerializeField] private State _stateDeath2Legs;
        [SerializeField] private State _stateDeath4Legs;
        [SerializeField] private EffectBase[] _effectsOnDeath;
        [SerializeField] private ObscuredInt _goldForKill;
        [SerializeField] private ObscuredInt _blueprintForKill;

#pragma warning restore 0649
        #endregion

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public TargetDetection TargetDetection => _targetDetection;
        public StateProcessor StateProcessor => _stateProcessor;
        public State StateHit2Legs => _stateHit2Legs;
        public State StateHit4Legs => _stateHit4Legs;
        public State StateDeath2Legs => _stateDeath2Legs;
        public State StateDeath4Legs => _stateDeath4Legs;
        public Interrupt[] Interrupts => _interrupts;
        public ConditionBase CanGotHit => _canGotHit;
        public EffectBase EffectCantGotHit => _effectCantGotHit;
        public int GoldForKill => _goldForKill;
        public int BlueprintForKill => _blueprintForKill;
        private float random;

        public EffectBase[] EffectsOnDeath => _effectsOnDeath;
        public BoolParametr Is2Legs => _is2LegsParametr;
        
        private AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;


        public State State { get; private set; }

        private State StateHit => Is2Legs.Bool ? StateHit2Legs : StateHit4Legs;
        private State StateDeath => Is2Legs.Bool ? StateDeath2Legs : StateDeath4Legs;

        private void Awake()
        {
            random = Random.Range(0, 100);
        }

        void IDamageable.Damage(float value, GameObject from)
        {
            var fromTarget = from.GetComponent<Target>();
            Health.AdjustHealth(-value);

            if (StateProcessor.State != StateHit2Legs && StateProcessor.State != StateHit4Legs)
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
                    CoinObjectsModel.SpawnAtPosition(GoldForKill, transform.position, transform.position + Vector3.up, 2f, AnimalID.Bear.ToString());
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
                AnimalsModel.TargetKillAnimal(from.GetComponent<Target>(), AnimalID.Bear);
            }
            else
            {
                if (TargetDetection.Target != fromTarget)
                {
                    TargetDetection.SetTarget(fromTarget);
                }

                if (!State.IsLockInterruptTransition && CanGotHit.IsTrue)
                {
                    StateHit.SetNext(Interrupts);
                    StateProcessor.TransitionTo(StateHit);
                    EffectCantGotHit.Apply();
                }
            }
        }

        void OnEnable()
        {
            HitZoneManager.RegisterObjectGlobal(this);
        }

        void OnDisable()
        {
            HitZoneManager.UnregisterObjectGlobal(this);
        }

        [SerializeField]
        Vector3 targetShift = new Vector3(0f, 1.45f, 0.3f);

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(targetShift);
        }

        public bool IsUseWeaponRange()
        {
            return true;
        }

        [SerializeField]
        List<Renderer> _renderers;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public int GetColor()
        {
            return 0;
        }
    }
}
