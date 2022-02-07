using System.Collections.Generic;
using ActionsCollections;
using DebugRelated;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DebugOptionsActionFlyCamera : ActionBase
    {
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private string _operationName;

        private bool _on;
        private FlyCamera _flyCamera;
        private float _speed;

        public DebugOptionsActionFlyCamera(string name, float speed)
        {
            _operationName = name;
            _speed = speed;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            _on = !_on;
            if(_on)
            {
                PlayerEventHandler.GetComponent<CharacterController>().enabled = false;

                if(_flyCamera == null)
                {
                    _flyCamera = PlayerEventHandler.gameObject.AddComponent<FlyCamera>();
                    _flyCamera.speed = _speed;
                }
                else
                {
                    _flyCamera = PlayerEventHandler.GetComponent<FlyCamera>();
                    _flyCamera.enabled = true;
                }
            }
            else
            {
                PlayerEventHandler.GetComponent<CharacterController>().enabled = true;
                if(_flyCamera == null) return;
                
                GameObject.Destroy(_flyCamera);

                var newRotation = new Vector3(0, 0, 0);

                PlayerEventHandler.transform.rotation = Quaternion.Euler(newRotation);
            }
        }

    }
}