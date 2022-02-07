using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class AIActionSpawnEnemiesSet : ActionBase
    {
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private EnemiesProvider EnemiesProvider => ModelsSystem.Instance.enemiesProvider;
        private string _operationName;
        private EnemyID _enemy;

        public AIActionSpawnEnemiesSet(string name, EnemyID enemy)
        {
            _operationName = name;
            _enemy = enemy;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            Initable initable = EnemiesProvider[_enemy];

            Initable initableInstance = GameObject.Instantiate(initable);
            
            initableInstance.Init();
        }

    }
}