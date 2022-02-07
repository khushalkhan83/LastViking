using System.Collections.Generic;
using Game.AI;
using Game.AI.BehaviorDesigner;
using Game.Controllers;
using Game.Models;
using MarchingBytes;
using UnityEngine;

namespace Encounters
{
    public class EnemyFactory
    {
        public Enemy GetEnemy(Transform soket, EnemyConfig config, bool doFarAwayChecks, List<GameObject> wayPoints)
        {
            return GetEnemy(soket.position,soket.transform,config,doFarAwayChecks,wayPoints);
        }

        public Enemy GetEnemy(Vector3 position, Transform parrent, EnemyConfig config, bool doFarAwayChecks, List<GameObject> wayPoints)
        {
            var instance = GetEnemyInstance(position, parrent, config.id);
            var health = instance.GetComponentInChildren<IHealth>();
            var context = instance.GetComponentInChildren<EnemyContext>();
            var initable = instance.GetComponentInChildren<Initable>();
            var damageReciver = instance.GetComponentInChildren<EnemyDamageReciver>();
            var selfTarget = instance.GetComponentInChildren<Target>();
            DistanceFromPlayerChecker farAwayCheck = null;


            if(doFarAwayChecks)
            {
                context.gameObject.TryGetComponent<DistanceFromPlayerChecker>(out var cheker);
                if(cheker == null)
                {
                    cheker = context.gameObject.AddComponent<DistanceFromPlayerChecker>();
                }

                farAwayCheck = cheker;
                farAwayCheck.enabled = true;
                // this component do farAwayCheck.enabled = false when gameobject or its root disabled
            }

            DeathController deathController = instance.gameObject.GetComponent<DeathController>();
            if (deathController == null)
            {
                deathController = instance.gameObject.AddComponent<DeathController>();
            }

            var regen = instance.GetComponentInChildren<EnemyRegenHP>();
            if(regen != null)
            {
                regen.enabled = regen;
            }
            else
            {
                Debug.LogError($"No valid component on this PREFAB {config.id}");
            }

            var answer = new Enemy(health, context, initable, deathController, config, damageReciver, farAwayCheck,wayPoints,selfTarget);

            return answer;
        }

        private GameObject GetEnemyInstance(Vector3 position, Transform parrent, EnemyID id)
        {
            return EasyObjectPool.instance.GetObjectFromPool(id.ToString(), position, Quaternion.identity, parrent);
        }
    }
}