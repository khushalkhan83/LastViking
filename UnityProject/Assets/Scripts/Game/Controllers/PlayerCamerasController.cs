using System;
using Cinemachine;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerCamerasController : IPlayerCamerasController, IController
    {
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }
        [Inject] public PlayerCameras PlayerCameras { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public ViewsSystemCameraModel ViewsSystemCameraModel { get; private set; }
        [Inject] public CinematicCameraModel CinematicCameraModel { get; private set; }

        private Camera CameraWorld => PlayerCameras.CameraWorld;
        private CinemachineFreeLook Camera => PlayerCameras.Camera;
        private CinemachineBrain Brain => PlayerCameras.Brain;

        void IController.Enable()
        {
            ViewsStateModel.OnIsHUDBlockedChanged += ProcessState;
            CinematicModel.OnCinematicActiveStateChanged += ProcessState;
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            ViewsStateModel.OnIsHUDBlockedChanged -= ProcessState;
            CinematicModel.OnCinematicActiveStateChanged -= ProcessState;
        }


        private void ProcessState()
        {
            bool cinematicActive = CinematicModel.CinematicStarted;
            bool playerIsDeadState = PlayerHealthModel.IsDead;

            ViewsSystemCameraModel.Camera.enabled = !cinematicActive;
            CinematicCameraModel.SetCameraActive(cinematicActive);


            if (cinematicActive)
            {
                Camera.enabled = true;
                CameraWorld.enabled = true;
                return;
            }

            if (playerIsDeadState)
            {
                Camera.enabled = false;
                CameraWorld.enabled = false;
                return;
            }

            bool hubBlockedByWindow = !ViewsStateModel.IsHUDBlocked_ByWindows;

            CameraWorld.enabled = hubBlockedByWindow;
            Camera.enabled = hubBlockedByWindow;
            Brain.enabled= hubBlockedByWindow;
        }
    }
}
