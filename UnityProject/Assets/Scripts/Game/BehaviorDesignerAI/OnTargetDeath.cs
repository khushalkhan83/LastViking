using Game.AI;
using Game.Models;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public class OnTargetDeath : MonoBehaviour
    {

        #region Data
    #pragma warning disable 0649

        [SerializeField] private Attack _attack;
        [SerializeField] private AttackRange _attackRange;
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private AnimalID _animalID;
        [SerializeField] private EnemyID _enemyID;

    #pragma warning restore 0649
        #endregion

        public Attack Attack => _attack;
        public AttackRange AttackRange => _attackRange;
        private EnemyType EnemyType => _enemyType;
        public AnimalID AnimalID => _animalID;
        public EnemyID EnemyID => _enemyID;
        public EnemiesModel EnemiesModel => ModelsSystem.Instance._enemiesModel;
        public AnimalsModel AnimalsModel => ModelsSystem.Instance._animalsModel;


        private void OnEnable()
        {
            if(Attack != null)
                Attack.OnKillTarget += OnKillTarget;
            if(AttackRange != null)
                AttackRange.OnKillTarget += OnKillTarget;
        }

        private void OnDisable()
        {
            if(Attack != null)
                Attack.OnKillTarget -= OnKillTarget;
            if(AttackRange != null)
                AttackRange.OnKillTarget -= OnKillTarget;
        }

        private void OnKillTarget(Target target) 
        {   
            if(EnemyType == EnemyType.Animal)
            {
                AnimalsModel.AnimalKillTarget(AnimalID, target);
            }
            else if(EnemyType == EnemyType.Enemy)
            {
                EnemiesModel.EnemyKillTarget(EnemyID, target);
            }
        }
    }
}