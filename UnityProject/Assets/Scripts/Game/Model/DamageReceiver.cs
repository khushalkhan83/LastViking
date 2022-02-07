using UnityEngine;

namespace Game.Models
{
    public class DamageReceiver : MonoBehaviour, IDamageable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private PlayerTargetItem _id;

#pragma warning restore 0649
        #endregion

        private IHealth _health;
        public IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public PlayerTargetItem ID => _id;

        void IDamageable.Damage(float value, GameObject from)
        {
            Health.AdjustHealth(-value);
        }
    }
}
