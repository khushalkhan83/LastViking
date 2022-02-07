using UnityEngine;
using UnityEngine.UI;

namespace UltimateSurvival
{
    public class GenericVitals : EntityBehaviour
    {

        #region Data
#pragma warning disable 0649

        [Header("Health & Damage")]

        [Tooltip("The health to start with.")]
        [SerializeField] private float m_MaxHealth = 100f;

        [SerializeField] private StatRegenData m_HealthRegeneration;

        [Range(0f, 1f)]
        [Tooltip("0 -> the damage received will not be decreased, \n1 -> the damage will be reduced to 0 (GOD mode).")]
        [SerializeField] private float m_Resistance = 0.1f;

        [Header("Audio")]

        [SerializeField] protected AudioSource m_AudioSource;

#pragma warning restore 0649
        #endregion

        protected float m_HealthDelta;
        private float m_NextRegenTime;

        public Image hpImage;

        public float MaxHealth => m_MaxHealth;

        private void Start()
        {
            Entity.Health.Set(MaxHealth);
            Entity.ChangeHealth.SetTryer(Try_ChangeHealth);
        }

        protected virtual void Update()
        {
            if (m_HealthRegeneration.CanRegenerate && Entity.Health.Value < MaxHealth && Entity.Health.Value > 0f)
            {
                var data = new HealthEventData(m_HealthRegeneration.RegenDelta);
                Entity.ChangeHealth.Try(data);
            }
        }

        protected virtual bool Try_ChangeHealth(HealthEventData healthEventData)
        {
            if (Entity.Health.Value == 0f)
                return false;
            if (healthEventData.Delta > 0f && Entity.Health.Value == MaxHealth)
                return false;

            float healthDelta = healthEventData.Delta;
            if (healthDelta < 0f)
                healthDelta *= (1f - m_Resistance);

            float newHealth = Mathf.Clamp(Entity.Health.Value + healthDelta, 0f, MaxHealth);
            Entity.Health.Set(newHealth);


            if (hpImage != null)
            {
                hpImage.fillAmount = newHealth / MaxHealth;
            }

            if (healthDelta < 0f)
                m_HealthRegeneration.Pause();


            return true;
        }
    }
}