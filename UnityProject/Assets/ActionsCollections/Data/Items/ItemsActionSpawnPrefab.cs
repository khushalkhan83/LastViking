using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class ItemsActionSpawnPrefab : ActionBase
    {
        private PlayerEventHandler Player => ModelsSystem.Instance._playerEventHandler;

        private string _operationName;
        private GameObject _prefab;
        private float _distanceFromPlayer;

        public ItemsActionSpawnPrefab(string name, GameObject prefab, float distanceFromPlayer)
        {
            _operationName = name;
            _prefab = prefab;
            _distanceFromPlayer = distanceFromPlayer;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            var playerTransform = Player.transform;
            var playerPosition = playerTransform.position;
            var playerRotation = playerTransform.rotation;

            

            var instance = GameObject.Instantiate(_prefab, playerTransform);

            instance.transform.localPosition += Vector3.forward * _distanceFromPlayer;

            instance.transform.SetParent(null);
        }
    }
}