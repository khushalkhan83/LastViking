using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugOptionsActionFlyCameraChangeSpeed : ActionBase
    {
        
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private string _operationName;

        private float _increment;

        public DebugOptionsActionFlyCameraChangeSpeed(string name, float increment)
        {
            _operationName = name;
            _increment = increment;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            var flyCam = PlayerEventHandler.GetComponent<DebugRelated.FlyCamera>();
            if(flyCam == null) return;

            flyCam.speed += _increment;
        }

    }
}