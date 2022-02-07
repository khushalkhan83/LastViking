using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugOptionsActionSpawnAnimal : ActionBase
    {
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private AnimalsProvider AnimalsProvider => ModelsSystem.Instance.animalsProvider;
        private string _operationName;
        private AnimalID _animal;
        private float _distanceFromPlayer;

        public DebugOptionsActionSpawnAnimal(string name, AnimalID animal, float distanceFromPlayer)
        {
            _operationName = name;
            _animal = animal;
            _distanceFromPlayer = distanceFromPlayer;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            Initable initable = AnimalsProvider[_animal];

            var playerTransform = PlayerEventHandler.transform;

            Initable initableInstance = GameObject.Instantiate(initable);
            
            initableInstance.transform.position = playerTransform.position;
            initableInstance.transform.localPosition += Vector3.forward * _distanceFromPlayer;

            initableInstance.Init();
        }

    }
}