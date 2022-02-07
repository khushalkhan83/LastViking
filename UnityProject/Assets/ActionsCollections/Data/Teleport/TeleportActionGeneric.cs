using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace DebugActions
{
    public class TeleportActionGeneric : ActionBase
    {
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;


        private string _operationName;
        private Vector3 _position;
        private Vector3 _rotation = Vector3.zero;
        
        public TeleportActionGeneric(string name, Vector3 position)
        {
            _operationName = name;
            _position = position;
        }
        public TeleportActionGeneric(string name, Vector3 position, Vector3 rot)
        {
            _operationName = name;
            _position = position;
            _rotation = rot;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            var player = PlayerEventHandler.gameObject;

            var newRotation = _rotation;

            player.GetComponent<CharacterController>().Move(Vector3.zero);
            player.transform.position = _position;
            player.transform.rotation = Quaternion.Euler(newRotation);
        }
    }
}