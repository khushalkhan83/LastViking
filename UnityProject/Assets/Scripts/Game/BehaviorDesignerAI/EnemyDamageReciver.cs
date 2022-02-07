using CodeStage.AntiCheat.ObscuredTypes;
using Game.Models;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;
using IDamageable = Game.Models.IDamageable;
using BehaviorDesigner.Runtime;
using Extensions;
using Game.ObjectPooling;

namespace Game.AI.BehaviorDesigner
{
    public class EnemyDamageReciver : MonoBehaviour, IDamageable, IHitReciver, IOutlineTarget, IResettable
    {

    #region Data
#pragma warning disable 0649
        [SerializeField] private ObscuredInt _goldForKill;
        [SerializeField] private ObscuredInt _blueprintForKill;

        [SerializeField] private GameObject _hitBox;
        [SerializeField] private GameObject _collider;
        [SerializeField] private Behaviour _nawMeshAgent;
        [SerializeField] private BehaviorTree _behaviourTree;

        [SerializeField] List< Renderer> _renderers;
        [SerializeField] GameObject _lootObject;
        [SerializeField] Vector3 _targetingShift;

        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private AnimalID _animalID;
        [SerializeField] private EnemyID _enemyID;

#pragma warning restore 0649
        #endregion

        private IHealth _health;
        private IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public int GoldForKill => _goldForKill;
        public int BlueprintForKill => _blueprintForKill;
        private float random;

        private AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;
        private EnemiesModel EnemiesModel => ModelsSystem.Instance._enemiesModel;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;
        private CoinObjectsModel CoinObjectsModel => ModelsSystem.Instance._coinObjectsModel;
        private BlueprintObjectsModel BlueprintObjectsModel => ModelsSystem.Instance._blueprintObjectsModel;

        public event Action<EnemyDamageReciver> OnReceiveDamage;
        public EnemyType EnemyType => _enemyType;
        public AnimalID AnimalID => _animalID;
        public EnemyID EnemyID => _enemyID;

        #region MonoBehaviour
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
        #endregion

        #region IResettable

        void IResettable.ResetObject()
        {
            _hitBox.SetActive(true);
            _nawMeshAgent.enabled = true;
            if(_collider != null) _collider.SetActive(true);
            if(_lootObject != null) _lootObject.SetActive(false);
        }
            
        #endregion


        #region IDamageable
        void IDamageable.Damage(float value, GameObject from)
        {
            var fromTarget = from.CheckNull()?.GetComponent<Target>();
            Health.AdjustHealth(-value);


            if (Health.IsDead)
            {
                _behaviourTree.SendEvent("Death");

                if (fromTarget != null && fromTarget.ID == TargetID.Player)
                {
                    StatisticsModel.Kill();
                    StatisticsModel.KillAnimal();
                }
                
                OnDeath();
        

                if (random <= 50f)
                {
                    CoinObjectsModel.SpawnAtPosition(GoldForKill, transform.position, transform.position + Vector3.up, 2f, EnemyID.ToString());
                }
                else
                {
                    BlueprintObjectsModel.SpawnAtPosition(BlueprintForKill, transform.position, transform.position + Vector3.up, 2f);
                }

                if(fromTarget != null)
                    TargetKillEnemy(fromTarget);
            }
            else
            {
                _behaviourTree.SendEvent("DamageRecived");
            }
            OnReceiveDamage?.Invoke(this);
        }
        #endregion

        #region IHitReciver
        Vector3 IHitReciver.GetPosition()
        {
            return transform.TransformPoint(_targetingShift);
        }
        #endregion


        #region IOutlineTarget

        public event Action<IOutlineTarget> OnUpdateRendererList;

        List<Renderer> IOutlineTarget.GetRenderers()
        {
            return _renderers;
        }

        bool IOutlineTarget.IsUseWeaponRange()
        {
            return true;
        }
        int IOutlineTarget.GetColor()
        {
            return 0;
        }
        #endregion

        private void TargetKillEnemy(Target target)
        {
            if(EnemyType == EnemyType.Animal)
            {
                AnimalsModel.TargetKillAnimal(target, AnimalID);
            }
            else if(EnemyType == EnemyType.Enemy)
            {
                EnemiesModel.TargetKillAnimal(target, EnemyID);
            }
        }

        private void OnDeath()
        {
            _hitBox.SetActive(false);
            //_nawMeshAgent.enabled = false;
            if(_collider != null) _collider.SetActive(false);
            if(_lootObject != null) _lootObject.SetActive(true);
        }
    }
}
