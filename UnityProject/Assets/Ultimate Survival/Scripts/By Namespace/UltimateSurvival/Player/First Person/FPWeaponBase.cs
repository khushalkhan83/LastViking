using Game.AI;
using System;
using UnityEngine;

namespace UltimateSurvival
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class RayImpact
    {
        [Range(0f, 1000f)]
        [SerializeField]
        [Tooltip("The damage at close range.")]
        private float m_MaxDamage = 15f;

        [Range(0f, 1000f)]
        [SerializeField]
        [Tooltip("The impact impulse that will be transfered to the rigidbodies at contact.")]
        private float m_MaxImpulse = 15f;

        [SerializeField]
        [Tooltip("How damage and impulse lowers over distance.")]
        private AnimationCurve m_DistanceCurve = new AnimationCurve(
            new Keyframe(0f, 1f),
            new Keyframe(0.8f, 0.5f),
            new Keyframe(1f, 0f));


        /// <summary>
        /// 
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="maxDistance"></param>
        public float GetDamageAtDistance(float distance, float maxDistance)
        {
            return ApplyCurveToValue(m_MaxDamage, distance, maxDistance);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>The impulse at distance.</returns>
        /// <param name="distance">Distance.</param>
        /// <param name="maxDistance">Max distance.</param>
        public float GetImpulseAtDistance(float distance, float maxDistance)
        {
            return ApplyCurveToValue(m_MaxImpulse, distance, maxDistance);
        }

        private float ApplyCurveToValue(float value, float distance, float maxDistance)
        {
            float maxDistanceAbsolute = Mathf.Abs(maxDistance);
            float distanceClamped = Mathf.Clamp(distance, 0f, maxDistanceAbsolute);

            return value * m_DistanceCurve.Evaluate(distanceClamped / maxDistanceAbsolute);
        }
    }

    public abstract class FPWeaponBase : FPObject
    {
        public Message Attack { get { return m_Attack; } }

        /// <summary></summary>
        public bool IsCanBeUsed { get; set; }

        private Message m_Attack = new Message();

        public HitInfo HitInfo { get; set; }


        [SerializeField, Range(0f, 100f)] protected float _kickback;

        public void SetHitInfo(HitInfo hitInfo) => HitInfo = hitInfo;

        public override void On_Draw(SavableItem correspondingItem) => base.On_Draw(correspondingItem);

        public virtual bool TryAttackOnce(Camera camera) { return false; }
        public virtual bool TryAttackContinuously(Camera camera) { return false; }

        protected void ChangeDurability(ItemProperty.Value durability)
        {
            if (EditorGameSettings.Instance.BreakWeaponImmediately)
            {
                durability.Float.Current = 0;
            }
            else
            {
                durability.Float.Current--;
            }
        }
    }
}
