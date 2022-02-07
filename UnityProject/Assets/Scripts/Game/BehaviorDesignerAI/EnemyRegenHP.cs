using System.Collections;
using System.Collections.Generic;
using BehaviorDesigner.Runtime;
using Game.Models;
using UnityEngine;

namespace Game.AI.BehaviorDesigner
{
    public class EnemyRegenHP : MonoBehaviour
    {
        [SerializeField] private BehaviorTree behaviorTree;
        [SerializeField] private EnemyHealthModel enemyHealthModel;
        [SerializeField] private float startRegenTime = 5f;
        [SerializeField] private float regenStepTime = 0.5f;
        [Range(0, 1f)]
        [SerializeField] private float regenStepValue = 0.1f;

        private SharedBool attackPlayer;
        private SharedBool attackShelter;
        private SharedBool runAway;

        private bool isEnemyPassive = true;
        private float startPassiveTime;
        private float statrRegenStepTime = 0;

        private void OnEnable() 
        {
            attackPlayer = (SharedBool)behaviorTree.GetVariable(SkeletonParams.AttackPlayer);
            attackShelter = (SharedBool)behaviorTree.GetVariable(SkeletonParams.AttackShelter);
            runAway = (SharedBool)behaviorTree.GetVariable(SkeletonParams.RunAway);
        }

        private void Update() 
        {
            if(enemyHealthModel.IsDead || enemyHealthModel.Health == enemyHealthModel.HealthMax)
                return;

            bool isInCombat = (attackPlayer != null && attackPlayer.Value) 
                            || (attackShelter != null && attackShelter.Value)
                            || (runAway != null && runAway.Value);

            if(isInCombat)
            {
                isEnemyPassive = false;
            }
            else if(!isInCombat && !isEnemyPassive)
            {
                isEnemyPassive = true;
                startPassiveTime = Time.time;
            }

            if(isEnemyPassive && Time.time - startPassiveTime > startRegenTime)
            {
                if(Time.time - statrRegenStepTime > regenStepTime)
                {
                    statrRegenStepTime = Time.time;
                    float health = Mathf.Clamp(enemyHealthModel.Health + enemyHealthModel.HealthMax * regenStepValue, 0 , enemyHealthModel.HealthMax);
                    enemyHealthModel.SetHealth(health);
                }
            }

        }
        
    }
}
