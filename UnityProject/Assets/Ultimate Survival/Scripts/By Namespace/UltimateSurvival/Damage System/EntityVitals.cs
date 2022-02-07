using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    /// <summary>
    /// 
    /// </summary>
    public class EntityVitals : GenericVitals
    {
        #region Data
#pragma warning disable 0649

        [Header("Fall Damage")]

        [Range(1f, 15f)]
        [Tooltip("At which landing speed, the entity will start taking damage.")]
        [SerializeField] private float m_MinFallSpeed = 4f;

        [Range(10f, 50f)]
        [Tooltip("At which landing speed, the entity will die, if it has no defense.")]
        [SerializeField] private float m_MaxFallSpeed = 15f;

        [Header("Audio")]

        [Tooltip("The sounds that will be played when this entity receives damage.")]
        [SerializeField] private SoundPlayer m_HurtAudio;

        [SerializeField] private float m_TimeBetweenScreams = 1f;

        [SerializeField] private SoundPlayer m_FallDamageAudio;

        [Header("Animation")]

        [SerializeField] private Animator m_Animator;

        [SerializeField] private float m_GetHitMax = 30f;

#pragma warning restore 0649
        #endregion

        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;


        private float m_NextTimeCanScream;

        public bool isNightEnemy;

        private bool isDay;

        private void Awake()
        {
            Entity.ChangeHealth.SetTryer(Try_ChangeHealth);
            Entity.Land.AddListener(On_Landed);
            isDay = false;
        }

        private void OnEnable()
        {
            Entity.Health.OnChange += OnChanged_Health;
        }

        private void OnDisable()
        {
            Entity.Health.OnChange -= OnChanged_Health;
        }

        private void Update()
        {
            if (isNightEnemy && GameTimeModel.TimeOfDay >= 5f && GameTimeModel.TimeOfDay < 19f && !isDay)
            {
                isDay = true;
                Invoke("KillEnemy", Random.Range(0, 4f));
            }
        }

        private void KillEnemy()
        {
            Entity.Health.Set(0);
        }

        private void OnChanged_Health()
        {
            float delta = Entity.Health.Value - Entity.Health.LastValue;
            if (delta < 0f)
            {
                if (m_Animator != null)
                {
                    m_Animator.SetFloat("Get Hit Amount", Mathf.Abs(delta / m_GetHitMax));
                    m_Animator.SetTrigger("Get Hit");
                }

                if (delta < 0f && Time.time > m_NextTimeCanScream)
                {
                    m_HurtAudio.Play(ItemSelectionMethod.RandomlyButExcludeLast, m_AudioSource);
                    m_NextTimeCanScream = Time.time + m_TimeBetweenScreams;
                }
            }
        }

        private void On_Landed(float landSpeed)
        {
            if (landSpeed >= m_MinFallSpeed)
            {
                Entity.ChangeHealth.Try(new HealthEventData(-100f * (landSpeed / m_MaxFallSpeed)));
                m_FallDamageAudio.Play(ItemSelectionMethod.Randomly, m_AudioSource);
            }
        }
    }
}
