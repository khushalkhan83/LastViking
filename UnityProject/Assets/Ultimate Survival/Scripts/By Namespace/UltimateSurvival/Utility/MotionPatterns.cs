using System;
using System.Collections;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class Sway
    {
        public Vector2 Value { get; private set; }

        #region Data
#pragma warning disable 0649

        [SerializeField] private bool Enabled;

        [SerializeField] private Vector2 Magnitude;

        [SerializeField] private float LerpSpeed;

#pragma warning restore 0649
        #endregion

        public Sway GetClone()
        {
            return (Sway)MemberwiseClone();
        }

        public void CalculateSway(Vector2 input, float deltaTime)
        {
            if (!Enabled)
                return;

            Value = Vector2.Lerp(Value, Vector3.Scale(Magnitude, input), deltaTime * LerpSpeed);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class TrigonometricBob
    {
        const float HORIZONTAL_SPEED = 1f;
        const float VERTICAL_SPEED = 2f;

        /// <summary> </summary>
        public float Time { get { return m_Time; } }

        #region Data
#pragma warning disable 0649

        [SerializeField] private bool m_Enabled = true;

        [Range(0.1f, 20f)]
        [Tooltip("How fast is the animation overall.")]
        [SerializeField] private float m_Speed = 0.18f;

        [Range(0.1f, 20f)]
        [Tooltip("How fast it blends out, when it's no longer used (so the transition between walk and run bobs are smooth for example).")]
        [SerializeField] private float m_CooldownSpeed = 5f;

        [Range(0f, 100f)]
        [SerializeField] private float m_AmountX = 0.2f;

        [Range(0f, 100f)]
        [SerializeField] private float m_AmountY = 0.2f;

        [Tooltip("You can control how fast the animation plays at different time intervals (time = 0 is the beginning, time = 1 is the end).")]
        [SerializeField] private AnimationCurve m_Curve = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(1f, 1f));

#pragma warning restore 0649
        #endregion

        private float m_Time;
        private Vector3 m_Vector;


        public TrigonometricBob GetClone()
        {
            return (TrigonometricBob)MemberwiseClone();
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 CalculateBob(float moveSpeed, float deltaTime)
        {
            if (!m_Enabled)
                return Vector3.zero;

            float x = Mathf.Sin(HORIZONTAL_SPEED * m_Time * Mathf.PI * 2f);
            float y = 1f - Mathf.Cos(VERTICAL_SPEED * m_Time * Mathf.PI * 2f);

            float incrementedTime = m_Time + m_Speed * moveSpeed * deltaTime * m_Curve.Evaluate(m_Time);
            m_Time = Mathf.Repeat(incrementedTime, 1f);

            m_Vector.x = x * m_AmountX;
            m_Vector.y = y * m_AmountY;

            return m_Vector;
        }

        /// <summary>
        /// 
        /// </summary>
        public Vector3 Cooldown(float deltaTime)
        {
            if (!m_Enabled)
                return Vector3.zero;

            m_Time = Mathf.Lerp(m_Time, GetBestCooldownValue(m_Time), deltaTime * m_CooldownSpeed);

            float x = Mathf.Sin(HORIZONTAL_SPEED * m_Time * Mathf.PI * 2f);
            float y = 1f - Mathf.Cos(VERTICAL_SPEED * m_Time * Mathf.PI * 2f);

            m_Vector.x = x * m_AmountX;
            m_Vector.y = y * m_AmountY;

            return m_Vector;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The best cooldown value.</returns>
        /// <param name="time">Time.</param>
        private float GetBestCooldownValue(float time)
        {
            float target = 0f;

            if (m_Time > 0.25f && m_Time < 0.5f)
                target = 0.5f;
            else if (m_Time > 0.5f && m_Time < 0.75f)
                target = 0.5f;
            else if (m_Time > 0.75f)
                target = 1f;

            return target;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class LerpControlledBob
    {
        /// <summary> </summary>
        public float Value { get; private set; }

        #region Data
#pragma warning disable 0649

        [SerializeField] private float m_MaxBobDuration;

        [SerializeField] private float m_MaxBobAmount;

        [SerializeField] private AnimationCurve m_DurationCurve;

        [SerializeField] private AnimationCurve m_DisplacementCurve;

#pragma warning restore 0649
        #endregion

        public IEnumerator DoBobCycle(float displacement)
        {
            // Make it move down slightly.
            float t = 0f;
            float bobDuration = m_MaxBobDuration * m_DurationCurve.Evaluate(displacement);
            float bobAmount = m_MaxBobAmount * m_DisplacementCurve.Evaluate(displacement);

            while (t < bobDuration)
            {
                Value = Mathf.Lerp(0f, bobAmount, t / bobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            // Make it move back to neutral.
            t = 0f;
            while (t < bobDuration)
            {
                Value = Mathf.Lerp(bobAmount, 0f, t / bobDuration);
                t += Time.deltaTime;
                yield return new WaitForFixedUpdate();
            }

            Value = 0f;
        }
    }
}
