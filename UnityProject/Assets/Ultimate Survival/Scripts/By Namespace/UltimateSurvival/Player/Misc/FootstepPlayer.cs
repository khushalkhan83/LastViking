using Game.Audio;
using Game.Models;
using Game.ThirdPerson;
using UnityEngine;

namespace UltimateSurvival
{
    public enum FootType { Left, Right }

    public class FootstepPlayer : PlayerBehaviour
    {

        #region Data
#pragma warning disable 0649

        [Header("General")]

        [SerializeField] private ItemSelectionMethod m_FootstepSelectionMethod;

        [SerializeField] private float m_LandSpeedThreeshold = 3f;

        [SerializeField] private LayerMask m_Mask;

        [SerializeField] private float _waterFootLevelOffset;

        [Header("Audio IDs")]
        [SerializeField] private AudioID[] _groundStepIds;
        [SerializeField] private AudioID[] _waterStepIds;
        [SerializeField] private AudioID[] _waterSwimIds;

        [Header("Distance Between Steps")]

        [SerializeField] private float m_WalkStepLength = 1.7f;

        [SerializeField] private float m_RunStepLength = 2f;

        [SerializeField] private float m_SwimStepLength = 4f;

        [Header("Volume Factors")]

        [SerializeField] private float m_WalkVolumeFactor = 0.5f;

        [SerializeField] private float m_RunVolumeFactor = 1f;
        [SerializeField] private CharacterLocomotion CharacterLocomotion;

#pragma warning restore 0649
        #endregion

        public AudioSystem AudioSystem => AudioSystem.Instance;

        private AudioSource m_LeftFootSource;
        private AudioSource m_RightFootSource;
        private FootType m_LastFroundedFoot;
        private float m_AccumulatedDistance;

        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;
        private PlayerMovementModel PlayerMovementModel => ModelsSystem.Instance._playerMovementModel;
        private PlayerLandModel PlayerLandModel => ModelsSystem.Instance._playerLandModel;

        [SerializeField] private CharacterController _playerCharacterController;
        public CharacterController PlayerCharacterController => _playerCharacterController;

        private void OnEnable()
        {
            PlayerMovementModel.OnChangeMovementID += OnChangePlayerMovement;
        }

        private void OnDisable()
        {
            PlayerMovementModel.OnChangeMovementID -= OnChangePlayerMovement;
        }

        private void Start()
        {
            // Make sure we get notified when the player jumps or lands.
            CharacterLocomotion.OnJump += OnStart_PlayerJump;
            // Player.Jump.AddStartListener(OnStart_PlayerJump);
            // Player.Land.AddListener(On_PlayerLanded);

            OnChangePlayerMovement();
        }

        private void FixedUpdate()
        {
            // Don't do anything if the player isn't grounded.
            if (!Player.IsGrounded.Value && PlayerMovementModel.MovementID == PlayerMovementID.Ground)
                return;

            // Update the distance accumulated based on the player's current speed.
            m_AccumulatedDistance += Player.Velocity.Value.magnitude * Time.fixedDeltaTime;

            // Get the step distance we should aim for, is different for crouching, walking and sprinting.
            float stepDistance = GetStepLength();

            // If we accumulated enough distance since the last footstep, play the sound and reset the counter.
            if (m_AccumulatedDistance >= stepDistance)
            {
                PlayFootstep();
                m_AccumulatedDistance = 0;
            }
        }

        private AudioID GetRandomAudioID(AudioID[] ids)
        {
            return ids[Random.Range(0, ids.Length)];
        }

        private AudioID GetSurfaceAudioID()
        {
            if (PlayerMovementModel.MovementID == PlayerMovementID.Ground)
            {
                if (IsOnWaterLine())
                    return GetRandomAudioID(_waterStepIds);
                else
                    return GetRandomAudioID(_groundStepIds);
            }
            else
                return GetRandomAudioID(_waterSwimIds);
        }


        private void OnChangePlayerMovement()
        {
            m_AccumulatedDistance = 0.9f * GetStepLength();
        }

        /// <summary>
        /// This method will play a footstep sound based on the selected method, and at a specific volume.
        /// </summary>
        private void PlayFootstep()
        {
            // Get the volumeFactor (it is different for crouching, walking and sprinting, for example when crouching the volume should be low).
            float volumeFactor = GetVolumeFactor();

            AudioSystem.PlayOnce(GetSurfaceAudioID());
        }


        /// <summary>
        /// Calculates the target step length based on whether we are walking, crouching or sprinting.
        /// </summary>
        private float GetStepLength()
        {
            float targetDistance = m_WalkStepLength;
            if (PlayerMovementModel.MovementID == PlayerMovementID.Ground)
            {
                if (PlayerRunModel.IsRun)
                    targetDistance = m_RunStepLength;
            }
            else
            {
                targetDistance = m_SwimStepLength;
            }

            return targetDistance;
        }

        /// <summary>
        /// Calculates the target volume based on whether player is walking, crouching or sprinting.
        /// </summary>
        private float GetVolumeFactor()
        {
            float targetVolume = m_WalkVolumeFactor;
            if (PlayerRunModel.IsRun)
                targetVolume = m_RunVolumeFactor;

            return targetVolume;
        }

        private void OnStart_PlayerJump()
        {
            AudioSystem.PlayOnce(AudioID.PlayerJump, transform.position);
        }

        private void On_PlayerLanded(float landSpeed)
        {
            bool landingWasHard = landSpeed >= m_LandSpeedThreeshold;
            if (landingWasHard)
            {
                AudioSystem.PlayOnce(AudioID.PlayerLanding, transform.position);
                m_AccumulatedDistance = 0f;
            }
        }

        private bool IsOnWaterLine() => PlayerCharacterController.transform.position.y > PlayerMovementModel.WaterLevel
            && PlayerCharacterController.transform.position.y <= PlayerMovementModel.WaterLevel + _waterFootLevelOffset;

        private SurfaceData GetCurrentData() => PlayerMovementModel.MovementID == PlayerMovementID.Water
            ? GetWaterData()
            : GetDataFromBelow();

        private SurfaceData GetWaterData() => GameController.SurfaceDatabase.GetWaterSurfaceData();

        private SurfaceData GetDataFromBelow()
        {
            if (!GameController.SurfaceDatabase)
            {
                Debug.LogWarning("No surface database found! can't play any footsteps...", this);
                return null;
            }

            var ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);
            var surfaceData = GameController.SurfaceDatabase.GetSurfaceData(ray, 1f, m_Mask);

            return surfaceData;
        }
    }
}

