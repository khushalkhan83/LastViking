using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace DebugActions
{
    public class ToggleObjectAction : ActionBase
    {
        private string _objectName;
        private string _operationName;


        private GameObject _inSceneObject;
        private GameObject InSceneObject
        {
            get
            {
                if (_inSceneObject == null)
                    _inSceneObject = GameObject.Find(_objectName);
                return _inSceneObject;
            }
        }

        public ToggleObjectAction(string name)
        {
            _operationName = name;
            _objectName = name;
        }
        public ToggleObjectAction(string operationName, string objectName)
        {
            _operationName = operationName;
            _objectName = objectName;
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
            InSceneObject.SetActive(!InSceneObject.activeSelf);
        }
    }
}