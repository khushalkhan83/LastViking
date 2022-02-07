using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;
using IDamageable = Game.Models.IDamageable;

namespace Game.AI.Behaviours.Chicken
{
    public class ChickenDamageReciver : MonoBehaviour, IDamageable, UltimateSurvival.IHitReciver, UltimateSurvival.IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private StateProcessor _stateProcessor;
        [SerializeField] private Interrupt[] _interrupts;
        [SerializeField] private State _stateHit;
        [SerializeField] private State _stateDeath;
        [SerializeField] private EffectBase[] _effectsOnDeath;
        [SerializeField] private ObscuredInt _goldForKill;
        [SerializeField] private ObscuredInt _blueprintForKill;
        [SerializeField] private List<Renderer> _renderers;

#pragma warning restore 0649
        #endregion

        public StateProcessor StateProcessor => _stateProcessor;
        public Interrupt[] Interrupts => _interrupts;
        public State StateHit => _stateHit;
        public State StateDeath => _stateDeath;
        public EffectBase[] EffectsOnDeath => _effectsOnDeath;
        public AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;
        public int GoldForKill => _goldForKill;
        public int BlueprintForKill => _blueprintForKill;
        private float random;

        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());
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
                    CoinObjectsModel.SpawnAtPosition(GoldForKill, transform.position, transform.position + Vector3.up, 2f, AnimalID.Chicken.ToString());
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
                AnimalsModel.TargetKillAnimal(from.GetComponent<Target>(), AnimalID.Chicken);
            }
            else
            {
                if (!State.IsLockInterruptTransition)
                {
                    StateProcessor.TransitionTo(StateHit);
                }
            }
        }

        Vector3 _chichenShift = Vector3.up * 0.5f;
        public Vector3 GetPosition()
        {
            return transform.TransformPoint(_chichenShift);
            //return transform.position;
        }

        public List<Renderer> GetRenderers()
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
