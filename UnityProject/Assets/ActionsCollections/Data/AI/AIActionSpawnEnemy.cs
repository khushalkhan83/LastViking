using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class AIActionSpawnEnemy : ActionBase
    {
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private EnemiesProvider EnemiesProvider => ModelsSystem.Instance.enemiesProvider;
        private string _operationName;
        private EnemyID _enemy;
        private float _distanceFromPlayer;

        public AIActionSpawnEnemy(string name, EnemyID enemy, float distanceFromPlayer)
        {
            _operationName = name;
            _enemy = enemy;
            _distanceFromPlayer = distanceFromPlayer;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            Initable initable = EnemiesProvider[_enemy];

            var playerTransform = PlayerEventHandler.transform;

            Initable initableInstance = GameObject.Instantiate(initable);
            
            initableInstance.transform.position = playerTransform.position;
            initableInstance.transform.localPosition += Vector3.forward * _distanceFromPlayer;

            initableInstance.Init();
        }

    }
}