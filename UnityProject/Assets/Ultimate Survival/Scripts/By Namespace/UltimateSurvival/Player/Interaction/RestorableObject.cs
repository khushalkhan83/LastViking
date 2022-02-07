using Game.Audio;
using Game.Models;
using UnityEngine;

namespace UltimateSurvival
{
    public class RestorableObject : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private RestorableID _id;
        [SerializeField] private AudioID _audioIDRestorable;

#pragma warning restore 0649
        #endregion
        public AudioID AudioIDRestorable => _audioIDRestorable;

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public RestorableID ID => _id;

        public void Restore(float healValue)//[REFACTOR][REMOVE]
        {
            Health.AdjustHealth(healValue);
            if (Health.Health > Health.HealthMax)
            {
                Health.AdjustHealth(Health.HealthMax - Health.Health);
            }
        }
    }
}
