using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class CinematicController : ICinematicController, IController
    {
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public PlayerCameras PlayerCameras { get; private set; }
        [Inject] public CinematicCameraModel CinematicCameraModel { get; private set; }
        [Inject] public ViewsSystemCameraModel ViewsSystemCameraModel { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public WorldCameraModel WorldCameraModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public ViewsInputModel ViewsInputModel { get; private set; }

        private bool godMode;
        private Camera oldCamera;

        void IController.Enable() 
        {
            CinematicModel.OnStartCinematic += OnStartCinematic;
            CinematicModel.OnEndCinematic += OnEndCinematic;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            CinematicModel.OnStartCinematic -= OnStartCinematic;
            CinematicModel.OnEndCinematic -= OnEndCinematic;
        }

        private void OnStartCinematic()
        {
            if(CinematicModel.CinematicStarted) return;

            CinematicModel.SetCinematicStarted(true);

            HandleCameras(true);
            BlockInput(true);

            oldCamera = WorldCameraModel.WorldCamera;

            godMode = PlayerHealthModel.IsCantRecieveDamage;
            if(!godMode)
            {
                PlayerHealthModel.CantRecieveDamage(true);
            }

            TryDisableBulidingMode();
        }

        private void OnEndCinematic()
        {
            if(!CinematicModel.CinematicStarted) return;

            HandleCameras(false);
            BlockInput(false);

            if(!godMode)
            {
                PlayerHealthModel.CantRecieveDamage(false);
            }

            TryDisableBulidingMode();

            CinematicModel.SetCinematicStarted(false);
        }

        private void TryDisableBulidingMode()
        {
            if(BuildingModeModel.BuildingActive)
                BuildingModeModel.BuildingActive = false;
        }


        private void HandleCameras(bool cinematicActive)
        {
            CinematicCameraModel.SetCameraActive(cinematicActive);
            PlayerCameras.CameraTools.enabled = !cinematicActive;
            ViewsSystemCameraModel.Camera.enabled = !cinematicActive;
        }

        private void BlockInput(bool active)
        {
            PlayerMovementModel.SetBlockPlayerMovement(active);
            TouchpadModel.SetBlockRotation(active);
            ViewsInputModel.BlockInput(active);
        }
    }
}