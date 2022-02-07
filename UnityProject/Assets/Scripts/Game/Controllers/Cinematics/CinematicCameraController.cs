using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class CinematicCameraController : ICinematicCameraController, IController
    {
        [Inject] public CinematicCameraModel Model { get; private set; }
        void IController.Enable() 
        {
            Model.OnActiveChanged += OnActiveChanged;

            Model.Init();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            Model.OnActiveChanged -= OnActiveChanged;
        }

        private void OnActiveChanged()
        {
            if(Model.CameraActive)
            {
                OnActivate();
            }
            else
            {
                OnDeactivate();
            }
        }

        private void OnActivate()
        {
            Model.CinemachineBrain.enabled = true;
        }

        private void OnDeactivate()
        {
            Model.CinemachineBrain.enabled = false;
            Model.WorldCamera.transform.localPosition = Vector3.zero;
            Model.WorldCamera.transform.localRotation = Quaternion.identity;
            Model.WorldCamera.fieldOfView = Model.PlayerCameraFOV;
        }
    }
}
