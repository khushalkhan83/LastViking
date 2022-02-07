using NaughtyAttributes;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    [CreateAssetMenu(fileName = "SO_config_enemy_new", menuName = "Configs/Enemies/simple", order = 0)]
    public class EnemyConfig : ScriptableObject
    {
        #region Data
#pragma warning disable 0649
        [ValidateInput("ValidateInput")] [SerializeField] private float health;
        [ValidateInput("ValidateInput")] [SerializeField] private float moveSpead;
        [ValidateInput("ValidateInput")] [SerializeField] private float attackDamage;
        [ValidateInput("ValidateInput")] [SerializeField] private float attackDistance;
        [ValidateInput("ValidateInput")] [SerializeField] private float timeBetweenAttack;
        [ValidateInput("ValidateInput")] [SerializeField] private float playerAgroDistance;
        [ValidateInput("ValidateInput")] [SerializeField] private float playerShelterModAgroDistance;
        [ValidateInput("ValidateInput")] [SerializeField] private float seekAriveDistance;
        [ValidateInput("ValidateInput")] [SerializeField] private float patrolRadius = 10f;
        [ValidateInput("ValidateInput")] [SerializeField] private float chaseTargetTime = 4f;
        [ValidateInput("ValidateInput")] [SerializeField] private float chaseDistance = 4f;

#pragma warning restore 0649
        #endregion

        public float Health => health;
        public float MoveSpead => moveSpead;
        public float AttackDamage => attackDamage;
        public float AttackDistance => attackDistance;
        public float TimeBetweenAttack => timeBetweenAttack;
        public float PlayerAgroDistance => playerAgroDistance;
        public float PlayerShelterModAgroDistance => playerShelterModAgroDistance;
        public float SeekAriveDistance => seekAriveDistance;
        public float PatrolRadius => patrolRadius;
        public float ChaseTargetTime => chaseTargetTime;
        public float ChaseDistance => chaseDistance;

        #region NaughtyAttributes
        private bool ValidateInput(float value)
        {
            return value > 0;    
        }
        #endregion
    }
}

