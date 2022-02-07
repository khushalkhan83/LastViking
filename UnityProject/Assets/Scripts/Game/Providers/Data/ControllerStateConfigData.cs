using Game.Controllers;
using RoboRyanTron.SearchableEnum;
using System;
using UnityEngine;

namespace Game.Providers
{
    [Serializable]
    public class ControllerStateConfigData
    {

        [SerializeField] private ControllerProcessingID _controllerProcessingID;
        [SearchableEnum]
        [SerializeField] private ControllerID[] _controllerIDs;

        public ControllerProcessingID ControllerProcessingID => _controllerProcessingID;
        public ControllerID[] ControllerIDs => _controllerIDs;
    }
}
