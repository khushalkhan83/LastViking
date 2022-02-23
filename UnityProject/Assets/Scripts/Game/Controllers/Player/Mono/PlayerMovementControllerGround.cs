using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerMovementControllerGround : MovementControllerBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CharacterController _characterController;
        [SerializeField] private PlayerEventHandler _playerEventHandler;
        [SerializeField] private float _acceleration;
        [SerializeField] private float _airAccelerationModifier;
        [SerializeField] private float _damping;
        [SerializeField] private float _forwardSpeed;
        [SerializeField] private float _sidewaysSpeed;
        [SerializeField] private float _backwardSpeed;
        [SerializeField] private float _antiBumpFactor;
        [SerializeField] private float _runSpeedMultiplier;
        [SerializeField] private float _jumpHeight;
        [SerializeField] private float _pushForce;
        [SerializeField] private float _gravity;
        [SerializeField] private float _slopeVelosityLimitForJump = 1f;

        [SerializeField] private AnimationCurve _slopeUp;
        [SerializeField] private AnimationCurve _slopeDown;

#pragma warning restore 0649
        #endregion

        public float Acceleration => _acceleration;
        public float AirAccelerationModifier => _airAccelerationModifier;
        public float Damping => _damping;
        public float ForwardSpeed => _forwardSpeed + _forwardSpeed * EquipmentBonus;
        public float SidewaysSpeed => _sidewaysSpeed + _sidewaysSpeed * EquipmentBonus;
        public float BackwardSpeed => _backwardSpeed + _backwardSpeed * EquipmentBonus;
        public float AntiBumpFactor => _antiBumpFactor;
        public float RunSpeedMultiplier => _runSpeedMultiplier;
        public float JumpHeight => _jumpHeight;
        public float PushForce => _pushForce;
        public float Gravity => _gravity;
        public float EquipmentBonus {get; private set;}
        public AnimationCurve SlopeUp => _slopeUp;
        public AnimationCurve SlopeDown => _slopeDown;
        public CharacterController CharacterController => _characterController;
        public PlayerEventHandler PlayerEventHandler => _playerEventHandler;
        private PlayerStaminaModel PlayerStaminaModel => ModelsSystem.Instance._playerStaminaModel;
        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;
        private JoystickModel JoystickModel => ModelsSystem.Instance._joystickModel;
        private PlayerLandModel PlayerLandModel => ModelsSystem.Instance._playerLandModel;
        private PlayerMovementModel PlayerMovementModel => ModelsSystem.Instance._playerMovementModel;

        private float MovementSpeedFactor => PlayerEventHandler.MovementSpeedFactor;
        private bool IsRun => PlayerRunModel.IsRun;

        protected bool IsJump { get; private set; }
        protected Vector3 HitNormal { get; private set; }

        private Vector3 _slopeVelosity;

        #region MonoBehaviour
        private void OnEnable()
        {
            PlayerEventHandler.Jump.AddStartTryer(TryStartJumpProcessing);
            PlayerEventHandler.Jump.OnStart += StartJumpHandler;
        }

        private void OnDisable()
        {
            PlayerEventHandler.Jump.OnStart -= StartJumpHandler;
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.rigidbody)
            {
                var forceMagnitude = PushForce * CharacterController.velocity.magnitude;
                var impactForce = (hit.moveDirection + Vector3.up * 0.35f) * forceMagnitude;
                hit.rigidbody.AddForceAtPosition(impactForce, hit.point);
            }

            HitNormal = hit.normal;
        }

        #endregion

        public void SetEquipmentPercentageBonus(float percentageBonus)
        {
            EquipmentBonus = percentageBonus / 100f;
        }

        private void StartJumpHandler() => IsJump = true;

        override public void Move(float deltaTime)
        {
            var input = Vector2.ClampMagnitude(JoystickModel.AxesNormalized, 1f);
            var velocity = GetVelocity(input, IsRun, deltaTime);

            if (IsJump)
            {
                velocity.y = GetJumpSpeed(JumpHeight);
                IsJump = false;
            }

            var isJump = PlayerEventHandler.Jump.Active;
            var isWalk = PlayerEventHandler.Walk.Active;
            var velocityLast = CharacterController.velocity;
            var velocitySqrMagnitude = velocityLast.sqrMagnitude;
            var isGrounded = CharacterController.isGrounded;
            var collisionFlags = CharacterController.Move(velocity * deltaTime);

            var isLand = (collisionFlags & CollisionFlags.Below) == CollisionFlags.Below && !PlayerEventHandler.IsGrounded.Value;
            var isJumpStop = isLand && isJump;
            var isWalkStart = isGrounded && !isJump && !isWalk && velocitySqrMagnitude > 0.1f && !IsRun;
            var isWalkStop = (isGrounded && !isJump && isWalk && (velocitySqrMagnitude < 0.1f || IsRun)) || (!isGrounded && isWalk);
            var isRunStop = (isGrounded && !isJump && IsRun && !PlayerStaminaModel.IsHasStamina) || (!isGrounded && IsRun);
            var isRunStopToggle = isGrounded && !isJump && IsRun && Vector3.Angle(transform.forward, velocityLast) > 90 || velocitySqrMagnitude < 0.1f;

            if (isWalkStart)
            {
                PlayerEventHandler.Walk.ForceStart();
            }

            if (isWalkStop)
            {
                PlayerEventHandler.Walk.ForceStop();
            }

            // if (isRunStop)
            // {
            //     PlayerRunModel.RunStop();
            // }

            // if (isRunStopToggle)
            // {
            //     PlayerRunModel.RunStop();
            //     PlayerRunModel.RunTogglePassive();
            // }

            if (isLand)
            {
                PlayerLandModel.OnPlayerLand(velocityLast);
            }

            if (isJumpStop)
            {
                PlayerEventHandler.Jump.ForceStop();
            }

            PlayerEventHandler.IsGrounded.Set(isGrounded);
            PlayerEventHandler.Velocity.Set(velocityLast);
            PlayerEventHandler.OnPreSaveHandler();
        }

        private Vector3 GetVelocity(Vector2 input, bool isRun, float deltaTime)
        {
            if (CharacterController.isGrounded)
            {
                return GetVelocityGround(input, isRun, deltaTime);
            }

            return GetVelocityFalling(input, isRun, deltaTime);
        }

        private Vector3 GetVelocityGround(Vector2 input, bool isRun, float deltaTime)
        {
            var speed = GetSpeed(input) * GetSpeedRunMul(input, isRun);
            var velocity = GetVelocityBase(input) * speed;
            var targetAccel = GetAcceleration(velocity, true);
            _slopeVelosity = GetSlope(velocity, HitNormal);

            if(!PlayerMovementModel.IgnoreSlopes)
                velocity += _slopeVelosity;

            velocity = Vector3.Lerp(CharacterController.velocity, velocity, targetAccel * deltaTime);

            velocity.y = -AntiBumpFactor;

            return velocity;
        }

        private Vector3 GetSlope(Vector3 velocity, Vector3 surfaceNormal)
        {
            var dir = new Vector3(velocity.x, 0, velocity.z);
            var dot = Vector3.Dot(dir.normalized, surfaceNormal);
            var slopeUp = SlopeUp.Evaluate(dot);
            var slopeDown = SlopeDown.Evaluate(Vector3.Dot(surfaceNormal, Vector3.up));
            var slopeDir = new Vector3(-velocity.x, slopeDown, -velocity.z);
            var dirSurface = new Vector3(surfaceNormal.x, 0, surfaceNormal.z);
            var dirNormal = dirSurface.normalized - surfaceNormal;

            return slopeDir * slopeUp + dirNormal * slopeDown;
        }

        private float GetAcceleration(Vector3 velocity, bool isGrounded)
        {
            float acceleration;
            if (velocity.sqrMagnitude > 0f)
            {
                acceleration = Acceleration;
            }
            else {

                acceleration = Damping;
            }

            if (!isGrounded) {

                acceleration = acceleration * AirAccelerationModifier;
            }

            return acceleration;
        }

        private Vector3 GetVelocityFalling(Vector2 input, bool isRun, float deltaTime)
        {
            var speed = GetSpeed(input) * GetSpeedRunMul(input, isRun);
            var velocity = GetVelocityBase(input) * speed;
            float velocityY = CharacterController.velocity.y;
            var targetAccel = GetAcceleration(velocity, false);
            velocity = Vector3.Lerp(CharacterController.velocity, velocity, targetAccel * deltaTime);



            velocity.y = velocityY - Gravity * Time.deltaTime;

            return velocity;
        }

        private float GetSpeedRunMul(Vector2 input, bool isRun)
        {
            if (isRun && input.y >= 0)
            {
                return RunSpeedMultiplier;
            }

            return 1;
        }

        private float GetSpeed(Vector2 input)
        {
            if (input.x == 0 && input.y == 0)
            {
                return 0;
            }

            if (input.y > 0)
            {
                return Mathf.Lerp(ForwardSpeed, SidewaysSpeed, Mathf.Abs(input.x)) * MovementSpeedFactor;
            }

            if (input.y < 0)
            {
                return Mathf.Lerp(BackwardSpeed, SidewaysSpeed, Mathf.Abs(input.x)) * MovementSpeedFactor;
            }

            return SidewaysSpeed * MovementSpeedFactor;
        }

        private Vector3 GetVelocityBase(Vector2 input)
        {
            if (input.x != 0 || input.y != 0)
            {
                return CharacterController.transform.TransformDirection(new Vector3(input.x, 0f, input.y));
            }

            return CharacterController.velocity.normalized;
        }

        private bool TryStartJumpProcessing()
        {
            bool slopeCondition = true;
            if(!PlayerMovementModel.IgnoreSlopes)
                slopeCondition = _slopeVelosity.sqrMagnitude <= _slopeVelosityLimitForJump * _slopeVelosityLimitForJump;

            return slopeCondition && PlayerStaminaModel.Stamina > 5f && PlayerEventHandler.IsGrounded.Value;
        }
            

        private float GetJumpSpeed(float heightToReach) => Mathf.Sqrt(2 * Gravity * heightToReach);
    }
}
