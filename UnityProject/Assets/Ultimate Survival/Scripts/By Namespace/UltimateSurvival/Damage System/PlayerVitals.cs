using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{

    public class PlayerVitals : EntityVitals
    {
        public PlayerEventHandler Player
        {
            get
            {
                if (!m_Player)
                    m_Player = GetComponent<PlayerEventHandler>();
                if (!m_Player)
                    m_Player = GetComponentInParent<PlayerEventHandler>();

                return m_Player;
            }
        }

        #region Data
#pragma warning disable 0649

        [Header("Stamina")]

        [Clamp(0f, 300f)]
        [SerializeField] private float m_StaminaDepletionRate = 30f;

        [SerializeField] private StatRegenData m_StaminaRegeneration;

        [SerializeField] private SoundPlayer m_BreathingHeavyAudio;

        [SerializeField] private float m_BreathingHeavyDuration = 11f;

        [Clamp(0f, 100f)]
        [SerializeField] private float m_JumpStaminaTake = 15f;

        [Header("Other")]
        [SerializeField] private  CharacterController _characterController;


#pragma warning restore 0649
        #endregion
        private PlayerStaminaModel PlayerStaminaModel => ModelsSystem.Instance._playerStaminaModel;

        private PlayerHealthModel PlayerHealthModel => ModelsSystem.Instance._playerHealthModel;
        private PlayerRunModel PlayerRunModel => ModelsSystem.Instance._playerRunModel;

        private CharacterController CharacterController => _characterController ?? (_characterController = GetComponent<CharacterController>());

        private PlayerEventHandler m_Player;
        private float m_LastHeavyBreathTime;

        protected override void Update()
        {
            base.Update();

            // Stamina.
            if (PlayerRunModel.IsRun && Vector3.Angle(transform.forward, CharacterController.velocity) <= 90 && CharacterController.velocity.magnitude > 0.5f)
            {
                ModifyStamina(-m_StaminaDepletionRate * Time.deltaTime);
            }

            if (!m_StaminaRegeneration.CanRegenerate && !PlayerStaminaModel.IsHasStamina && Time.time - m_LastHeavyBreathTime > m_BreathingHeavyDuration)
            {
                m_LastHeavyBreathTime = Time.time;
                m_BreathingHeavyAudio.Play2D();
            }
        }

        private void Start()
        {
            // HACK:

            Player.Jump.AddStartListener(() => ModifyStamina(-m_JumpStaminaTake));
        }

        private void ModifyStamina(float delta)
        {
            PlayerStaminaModel.AdjustStamina(delta);
        }
    }
}
