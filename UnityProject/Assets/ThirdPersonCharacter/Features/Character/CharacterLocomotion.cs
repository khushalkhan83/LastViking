using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.ThirdPerson
{
    public class CharacterLocomotion : MonoBehaviour
    {
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = 20;
        [SerializeField] private float stepDown = 0.1f;
        [SerializeField] private float airControl = 2.5f;
        [SerializeField] private float jumpDamp = 0.5f;
        [SerializeField] private float groundSpeed = 1;
        [SerializeField] private float maxGroundSpeed = 1f;
        [SerializeField] private float inputMultyplier = 1;


        [SerializeField] private float maxInputScale = 2;

        private PlayerStaminaModel PlayerStaminaModel => ModelsSystem.Instance._playerStaminaModel;
        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;


        [System.Serializable]
        private struct Settings
        {
            public float animationSpeed;
            public float movementSpeed;
        }

        public enum Preset { Aiming, Run, Walk }

        [Space]
        [SerializeField] private Preset preset;

        [SerializeField] private Settings aimingSettings;
        [SerializeField] private Settings runSettings;
        [SerializeField] private Settings walkSettings;

#if UNITY_EDITOR
        [SerializeField] private bool forbidExternalPresetChange = false;
#endif
        public void SetPreset(Preset preset)
        {
#if UNITY_EDITOR
            if (forbidExternalPresetChange) return;
#endif

            this.preset = preset;
        }

        private Settings GetSettings()
        {
            if (preset == Preset.Aiming) return aimingSettings;
            else if (preset == Preset.Run) return runSettings;
            return walkSettings;
        }

        private Animator animator;
        private Vector2 input;
        private Vector3 rootMotion;
        private Vector3 velocity;
        private bool isJump;
        private CharacterController characterController;
        private PlayerEventHandler PlayerEventHandler;

        private int hashInputX;
        private int hashInputY;
        private int hashIsJumping;

        private bool IsRun => PlayerRunModel.IsRun;

        private PlayerLandModel PlayerLandModel => ModelsSystem.Instance._playerLandModel;

        private void Start()
        {
            animator = GetComponent<Animator>();
            characterController = GetComponent<CharacterController>();
            PlayerEventHandler = GetComponent<PlayerEventHandler>();
            hashInputX = Animator.StringToHash("InputX");
            hashInputY = Animator.StringToHash("InputY");
            hashIsJumping = Animator.StringToHash("isJumping");
        }

        private void Update()
        {
            var scale = GetSettings().animationSpeed;
            input.x = Mathf.Clamp(PlayerInput.Instance.MoveInput.x * inputMultyplier, -scale, scale);
            input.y = Mathf.Clamp(PlayerInput.Instance.MoveInput.y * inputMultyplier, -scale, scale);

            animator.SetFloat(hashInputX, input.x);
            animator.SetFloat(hashInputY, input.y);

            if (PlayerInput.Instance.JumpInput)
            {
                Jump();
            }

            MoveUpdate();
        }

        private void OnAnimatorMove()
        {
            rootMotion += animator.deltaPosition;
        }

        private void MoveUpdate()
        {
            if (isJump)
            {
                UpdateOnAir();
            }
            else
            {
                UpdateOnGround();
            }
        }

        private void UpdateOnAir()
        {
            velocity.y -= gravity * Time.deltaTime;
            Vector3 displacement = velocity * Time.deltaTime;
            displacement += CalculateAirControl();
            characterController.Move(displacement);
            isJump = !characterController.isGrounded;
            rootMotion = Vector3.zero;
            animator.SetBool(hashIsJumping, isJump);

            //Player landed
            if (!isJump) PlayerLandModel.OnPlayerLand(velocity);
        }

        private void UpdateOnGround()
        {
            var isGrounded = characterController.isGrounded;
            var velocityLast = characterController.velocity;
            var velocitySqrMagnitude = velocityLast.sqrMagnitude;

            var isRunStop = (isGrounded && !isJump && IsRun && !PlayerStaminaModel.IsHasStamina) || (!isGrounded && IsRun);
            var isRunStopToggle = isGrounded && !isJump && IsRun && Vector3.Angle(transform.forward, velocityLast) > 90 || velocitySqrMagnitude < 0.1f;

            Vector3 stepForwardAmount = rootMotion * groundSpeed;
            Vector3 stepDownAmount = Vector3.down * stepDown;

            Vector3 movement = stepForwardAmount + stepDownAmount;

            var clampedMovement = Vector3.ClampMagnitude(movement, maxGroundSpeed);

            var speedModifier = GetSettings().movementSpeed;

            characterController.Move(clampedMovement * Time.deltaTime * speedModifier);
            rootMotion = Vector3.zero;

            if (isRunStop)
            {
                PlayerRunModel.RunStop();
            }
            if (isRunStopToggle)
            {
                PlayerRunModel.RunStop();
            }

            if (!characterController.isGrounded)
            {
                SetInAir(0);
            }

            PlayerEventHandler.IsGrounded.Set(isGrounded);
            PlayerEventHandler.Velocity.Set(velocityLast);
            PlayerEventHandler.OnPreSaveHandler();
        }

        private Vector3 CalculateAirControl()
        {
            return ((transform.forward * input.y) + (transform.right * input.x)) * (airControl / 100f);
        }

        private void Jump()
        {
            if (!isJump)
            {
                float jumpVelocity = Mathf.Sqrt(2 * gravity * jumpHeight);
                SetInAir(jumpVelocity);
            }
        }

        private void SetInAir(float jumpVelocity)
        {
            isJump = true;
            velocity = animator.velocity * jumpDamp * groundSpeed;
            velocity.y = jumpVelocity;
            animator.SetBool(hashIsJumping, true);
        }
    }
}
