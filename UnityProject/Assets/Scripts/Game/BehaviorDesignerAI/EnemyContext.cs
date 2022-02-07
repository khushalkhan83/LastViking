using UnityEngine;
using Game.Models;
using BehaviorDesigner.Runtime;
using Game.ObjectPooling;
using NaughtyAttributes;
using System.Collections.Generic;
using Extensions;

namespace Game.AI.BehaviorDesigner
{
    public class SkeletonParams
    {
        public const string Player = "Player";
        public const string AttackShelter = "AttackShelter";
        public const string AttackRange = "AttackRange";
        public const string AgroDistance = "AgroDistance";
        public const string AttackShelterModeAgroDistance = "AttackShelterModeAgroDistance";
        public const string RunSpeed = "RunSpeed";
        public const string TimeBetweenAttack = "TimeBetweenAttack";
        public const string AfraidPlayer = "AfraidPlayer";
        public const string PatrolPoints = "PatrolPoints";
        public const string OverrideTarget = "OverrideTarget";
        public const string SeekAriveDistance = "SeekAriveDistance";
        public const string PatrolRadius = "PatrolRadius";
        public const string ChaseTargetTime = "ChaseTargetTime";
        public const string ChaseDistance = "ChaseDistance";


        // ressetable on start params
        public const string HaveRightToAttackPlayer = "HaveRightToAttackPlayer";
        public const string AttackPlayer = "AttackPlayer";
        public const string RunAway = "RunAway";
    }

    public class EnemyContext : MonoBehaviour, IResettable // TODO: rename to enemy facade
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private List<GameObject> defaultPatrollPoints;
        [SerializeField] private EnemyConfig config;

        #pragma warning restore 0649
        #endregion

        private const bool attackShelter_defaultValue = false;
        private const bool affraidPlayer_defaulValue = false;
        private const bool attackPlayer_defaulValue = false;
        private const bool runAway_defaulValue = false;
        private const bool haveRightToAttackPlayer_defaulValue = true;

        private SharedBool haveRightToAttackPlayerVariable;
        private SharedBool attackPlayerVariable;
        private SharedBool afraidPlayerVariable;
        private BehaviorTree behaviorTree;
        
        public bool HaveRightToAttackPlayer
        {
            get => haveRightToAttackPlayerVariable.Value;
            private set => haveRightToAttackPlayerVariable.SetValue(value);
        }
        public bool AttackPlayer
        {
            get => attackPlayerVariable.Value;
            private set => attackPlayerVariable.SetValue(value);
        }
        public bool AfraidPlayer
        {
            get => afraidPlayerVariable.Value;
            private set => afraidPlayerVariable.SetValue(value);
        }


        #region Dependencies
        private GameObject PlayerGO => ModelsSystem.Instance._playerEventHandler.gameObject;
        private EnemiesModifierModel EnemiesModifierModel => ModelsSystem.Instance._enemiesModifierModel;
            
        #endregion

        #region MonoBehaviour

        private void Awake()
        {
            Init();
        }
        #endregion

        private bool inited;
        private void Init()
        {
            if(inited) return;

            behaviorTree = GetComponent<BehaviorTree>();

            if (behaviorTree == null) return;
            IniBehaviourTreeVariables();

            SetStaticParams();
            ResetDynamicParams();

            inited = true;
        }


        private void IniBehaviourTreeVariables()
        {
            haveRightToAttackPlayerVariable = (SharedBool)(behaviorTree.GetVariable(SkeletonParams.HaveRightToAttackPlayer));
            attackPlayerVariable = (SharedBool)(behaviorTree.GetVariable(SkeletonParams.AttackPlayer));
            afraidPlayerVariable = (SharedBool)(behaviorTree.GetVariable(SkeletonParams.AfraidPlayer));
        }

        private void ResetDynamicParams()
        {
            SetPatrolPoins(defaultPatrollPoints);
            SetRunAway(runAway_defaulValue);
            SetAttackPlayer(attackPlayer_defaulValue);
            SetAfraidPlayer(affraidPlayer_defaulValue);
            SetAttackShelter(attackShelter_defaultValue);
            SetHaveRightToAttackPlayer(haveRightToAttackPlayer_defaulValue);
            SetOverrideTarget(null);
        }

        private void SetStaticParams()
        {
            Bind(SkeletonParams.Player,PlayerGO);

            Bind(SkeletonParams.AttackShelterModeAgroDistance,config.PlayerShelterModAgroDistance);
            Bind(SkeletonParams.AgroDistance,config.PlayerAgroDistance);
            Bind(SkeletonParams.RunSpeed,config.MoveSpead * EnemiesModifierModel.SpeedScaler);
            Bind(SkeletonParams.TimeBetweenAttack,config.TimeBetweenAttack);
            Bind(SkeletonParams.AttackRange,config.AttackDistance);
            Bind(SkeletonParams.SeekAriveDistance,config.SeekAriveDistance);
            Bind(SkeletonParams.PatrolRadius,config.PatrolRadius);
            Bind(SkeletonParams.ChaseTargetTime,config.ChaseTargetTime);
            Bind(SkeletonParams.ChaseDistance, config.ChaseDistance);
        }

        #region IResettable
        public void ResetObject()
        {
            Init();

            ResetDynamicParams();
        }
            
        #endregion

        #region Debug

        [Header("Debug")]
        [SerializeField] private bool test_haveRightToAttackPlayer;
        [SerializeField] private bool test_haveRightToAttackShelter;
        [SerializeField] private bool test_setAfraidOfPlayer;
        [SerializeField] private Target test_target;

        [Button] void SetHaveRightToAttackPlayer() => SetHaveRightToAttackPlayer(test_haveRightToAttackPlayer);
        [Button] void SetAttackShelter() => SetAttackShelter(test_haveRightToAttackShelter);
        [Button] void SetAfraidPlayer() => SetAfraidPlayer(test_setAfraidOfPlayer);
        [Button] void SetOverrideTarget() => SetOverrideTarget(test_target);
        #endregion

        public void SetHaveRightToAttackPlayer(bool value) => HaveRightToAttackPlayer = value;
        public void SetAttackPlayer(bool value) => AttackPlayer = value;
        public void SetAfraidPlayer(bool value) => AfraidPlayer = value;

        public void SetAttackShelter(bool value) => Bind(SkeletonParams.AttackShelter,value);
        public void SetPatrolPoins(List<GameObject> patrolPoints) => Bind(SkeletonParams.PatrolPoints, patrolPoints);
        private void SetRunAway(bool value) => Bind(SkeletonParams.RunAway,value);
        public void SetOverrideTarget(Target target) => Bind(SkeletonParams.OverrideTarget, target.CheckNull()?.gameObject);

        private void Bind(string variableName, object value)
        {
            var variable = behaviorTree.GetVariable(variableName);
            if(variable != null) variable.SetValue(value);
        }

        private void Bind(SharedVariable variable, object value)
        {
            variable.SetValue(value);
        }
    }
}
