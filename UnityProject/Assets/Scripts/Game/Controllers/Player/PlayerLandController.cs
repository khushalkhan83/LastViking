using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerLandController : IPlayerLandController, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerLandModel PlayerLandModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }
        [Inject] public PlayerMovementControllerGround PlayerMovementControllerGround { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        private CharacterController characterController;

        void IController.Enable()
        {
            PlayerLandModel.OnLand += OnLandHandler;
            GameUpdateModel.OnUpdate += UpdateNotGroundedTime;
            characterController = PlayerEventHandler.GetComponent<CharacterController>();
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerLandModel.OnLand -= OnLandHandler;
            GameUpdateModel.OnUpdate -= UpdateNotGroundedTime;
        }


        private void UpdateNotGroundedTime()
        {
            bool isGrounded = characterController.isGrounded;
            if(!isGrounded) PlayerLandModel.UpdateNotGroundedTime();
        }

        private void OnLandHandler()
        {
            float landSpeed = PlayerLandModel.Velocity.magnitude;

            float damageValue = PlayerLandModel.DamageCurve.Evaluate(landSpeed);
            float damage = PlayerHealthModel.HealthCurrentMax * damageValue;

            if (damage > 0 && PlayerLandModel.CanTakeLandedDamage)
            {
                AudioSystem.PlayOnce(AudioID.PlayerTakeHit01);
                PlayerHealthModel.AdjustHealth(-damage);
            }

            PlayerLandModel.SetLanded();
        }
    }
}
