using System;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerMovementController : IPlayerMovementController, IController
    {
        [Inject] public PlayerMovementControllerGround MovementControllerGround { get; private set; }
        [Inject] public PlayerMovementControllerWater MovementControllerWater { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        private MovementControllerBase MovementController { get; set; }

        private FPMotion FPMotion 
        {
            get
            {
                // TODO: debug for many instances (in case with fising (main weapon is disabled. and fishing rod is enabled))
                return PlayerEventHandler.GetComponentInChildren(typeof(FPMotion),true) as FPMotion;
            }
        }

        private FootstepPlayer _footstepPlayer;
        private FootstepPlayer FootstepPlayer
        {
            get
            {
                if(_footstepPlayer == null) _footstepPlayer = PlayerEventHandler.GetComponent<FootstepPlayer>();
                return _footstepPlayer;
            }
        }

        void IController.Enable()
        {
            // PlayerMovementModel.OnInitializeData += OnInitializeDataHandler;
            // PlayerMovementModel.OnChangeMovementID += OnChangeMovementIDHandler;
            // PlayerMovementModel.OnBlockPlayerMovement += OnBlockPlayerMovement;
            // GameUpdateModel.OnUpdate += OnUpdate;
            // SetMovement(PlayerMovementModel.MovementID);
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            // PlayerMovementModel.OnInitializeData -= OnInitializeDataHandler;
            // PlayerMovementModel.OnChangeMovementID -= OnChangeMovementIDHandler;
            // PlayerMovementModel.OnBlockPlayerMovement -= OnBlockPlayerMovement;
            // GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if (Time.deltaTime > 0)
            {
                if(PlayerMovementModel.BlockPlayerMovement) return;

                MovementController?.Move(Time.deltaTime);
            }
        }

        private void OnInitializeDataHandler()
        {
            PlayerMovementModel.OnInitializeData -= OnInitializeDataHandler;
            SetMovement(PlayerMovementModel.MovementID);
        }

        protected MovementControllerBase GetMovementController(PlayerMovementID playerMovementID)
        {
            switch (playerMovementID)
            {
                case PlayerMovementID.Ground:
                    return MovementControllerGround;
                case PlayerMovementID.Water:
                    return MovementControllerWater;
            }

            return null;
        }

        private void OnChangeMovementIDHandler() => SetMovement(PlayerMovementModel.MovementID);

        private void SetMovement(PlayerMovementID movementID) => MovementController = GetMovementController(movementID);


        private void OnBlockPlayerMovement(bool block)
        {
            if(FPMotion != null)
                FPMotion.enabled = !block;
            
            if(FootstepPlayer != null)
                FootstepPlayer.enabled = !block;
        }
    }
}
