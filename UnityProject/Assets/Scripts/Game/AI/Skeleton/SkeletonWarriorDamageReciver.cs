using CodeStage.AntiCheat.ObscuredTypes;
using Core.StateMachine;
using Core.States.Parametrs;
using Game.Models;
using Game.StateMachine.Parametrs;
using System;
using UnityEngine;
using Random = UnityEngine.Random;
using IDamageable = Game.Models.IDamageable;
using UltimateSurvival;
using System.Collections.Generic;

namespace Game.AI.Behaviours.Skeleton
{
    public class SkeletonWarriorDamageReciver : MonoBehaviour, IDamageable, IHitReciver, IOutlineTarget
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private TargetDetection _targetDetection;
        [SerializeField] private PositionParametr _hitPoint;
        [SerializeField] private PositionParametr _hitDirection;
        [SerializeField] private FloatParametr _hitForce;
        [SerializeField] private StateProcessor _stateProcessor;
        [SerializeField] private State _stateHit;
        [SerializeField] private Interrupt[] _interrupts;
        [SerializeField] private State _stateDeath;
        [SerializeField] private EffectBase _effectOnDeath;
        [SerializeField] private EnemyID _enemyID;
        [SerializeField] private ObscuredInt _goldForKill;
        [SerializeField] private ObscuredInt _blueprintForKill;

#pragma warning restore 0649
        #endregion

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public TargetDetection TargetDetection => _targetDetection;
        public StateProcessor StateProcessor => _stateProcessor;
        public State StateHit => _stateHit;
        public State StateDeath => _stateDeath;
        public Interrupt[] Interrupts => _interrupts;
        public Interrupt InterruptDefault => _interrupts[_interrupts.Length - 1];
        public EffectBase EffectOnDeath => _effectOnDeath;
        public EnemyID EnemyID => _enemyID;
        public int GoldForKill => _goldForKill;
        public int BlueprintForKill => _blueprintForKill;
        private float random;

        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;
        public EnemiesModel EnemiesModel => ModelsSystem.Instance._enemiesModel;

        public State State { get; private set; }

        private void Awake()
        {
            random = Random.Range(0, 100);
            //Array.Resize(ref _interrupts, _interrupts.Length + 1);
            //_interrupts[_interrupts.Length - 1] = new Interrupt();
        }

        void IDamageable.Damage(float value, GameObject from)
        {
            Health.AdjustHealth(-value);

            var fromTarget = from.GetComponent<Target>();
            var hitInfo = from.GetComponent<HitInfo>();

            _hitPoint.Position = hitInfo.HitPoint;
            _hitDirection.Position = hitInfo.HitDirection;
            _hitForce.SetValue(hitInfo.Kickback);

            //InterruptDefault.SetState(StateProcessor.State);

            if (StateProcessor.State != StateHit)
            {
                State = StateProcessor.State;
            }

            if (Health.IsDead)
            {
                if (fromTarget.ID == TargetID.Player)
                {

                    if (random <= 50f)
                    {
                        CoinObjectsModel.SpawnAtPosition(GoldForKill, transform.position, transform.position + Vector3.up, 2f, EnemyID.ToString());
                    }
                    else
                    {
                        BlueprintObjectsModel.SpawnAtPosition(BlueprintForKill, transform.position, transform.position + Vector3.up, 2f);
                    }

                    StatisticsModel.Kill();
                    StatisticsModel.KillSkeleton();
                }

                EffectOnDeath.Apply();
                StateProcessor.TransitionTo(StateDeath);
                EnemiesModel.TargetKillAnimal(from.GetComponent<Target>(), EnemyID);
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
                    //StateHit.SetNext(InterruptDefault);
                    StateHit.SetNext(Interrupts);
                }

                if (!State.IsLockInterruptTransition)
                {
                    StateProcessor.TransitionTo(StateHit);
                }
            }
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

        void OnEnable()
        {
            HitZoneManager.RegisterObjectGlobal(this);
        }

        void OnDisable()
        {
            HitZoneManager.UnregisterObjectGlobal(this);
        }

        [SerializeField]
        Vector3 targetShift;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public Vector3 GetPosition()
        {
            return transform.TransformPoint(targetShift);
        }
        public int GetColor()
        {
            return 0;
        }
    }
}
