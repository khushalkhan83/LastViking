using System;
using UnityEngine;

namespace Game.Models
{
    public class PlayerLandModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _landingDamageMultiplier;
        [SerializeField] private AnimationCurve _damageCurve;
        [SerializeField] private float _fallTimeWithoutDamage = 0.5f;

#pragma warning restore 0649
        #endregion

        public float LandingDamageMultiplier => _landingDamageMultiplier;
        public AnimationCurve DamageCurve => _damageCurve;

        public Vector3 Velocity { get; private set; }

        public event Action OnLand;

        public void OnPlayerLand(Vector3 velocity)
        {
            Velocity = velocity;
            OnLand?.Invoke();
        }

        public float NotGroundedTime {get;set;}

        public bool CanTakeLandedDamage
        {
            get => NotGroundedTime > _fallTimeWithoutDamage;
        }

        public void UpdateNotGroundedTime()
        {
            NotGroundedTime += Time.unscaledDeltaTime;
        }

        public void SetLanded()
        {
            NotGroundedTime = 0;
        }
    }
}