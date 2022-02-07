using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerMovementControllerWater : MovementControllerBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private float _speed;
        [SerializeField] private float _runSpeedMultiplier;
        [SerializeField] private float _yThreshold;

#pragma warning restore 0649
        #endregion

        public CharacterController CharacterController => _characterController;
        private JoystickModel JoystickModel => ModelsSystem.Instance._joystickModel;
        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;
        private PlayerMovementModel PlayerMovementModel => ModelsSystem.Instance._playerMovementModel;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private PlayerStaminaModel PlayerStaminaModel => ModelsSystem.Instance._playerStaminaModel;

        public float Speed => _speed;
        public float RunSpeedMultiplier => _runSpeedMultiplier;
        public float YThreshold => _yThreshold;

        private bool IsRun => PlayerRunModel.IsRun;

        override public void Move(float deltaTime)
        {
            var isRunStop = IsRun && !PlayerStaminaModel.IsHasStamina;

            if (isRunStop)
            {
                PlayerRunModel.RunStop();
            }

            var input = JoystickModel.AxesNormalized;
            var speed = GetSpeed(IsRun);

            if (CharacterController.transform.position.y < PlayerMovementModel.WaterLevel - YThreshold)
            {
                var position = CharacterController.transform.position;
                position.y = PlayerMovementModel.WaterLevel - YThreshold / 2;

                CharacterController.transform.position = position;
            }

            CharacterController.Move((CharacterController.transform.forward * input.y + CharacterController.transform.right * input.x) * speed * deltaTime);
            PlayerEventHandler.Velocity.Set(CharacterController.velocity);
        }

        private float GetSpeed(bool isRun)
        {
            if (isRun)
            {
                return Speed * RunSpeedMultiplier;
            }

            return Speed;
        }
    }
}
