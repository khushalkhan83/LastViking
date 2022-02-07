using System;
using UnityEngine;

namespace Game.Models
{
    public class FishHealthModel : MonoBehaviour
    {
        [SerializeField, Tooltip("min fish health")]
        float _minFishHealth = 50f;
        [SerializeField, Tooltip("max fish health")]
        float _maxFishHealth = 150f;
        [SerializeField, Tooltip("Fish full health")]
        float _fullFishHealth = 100f;
        [SerializeField, Tooltip("Damage to fish")]
        float _damage = 10f;
        [SerializeField, Tooltip("Time between damage")]
        float _damageDelay = 1f;
        [SerializeField, Tooltip("Fish health")]
        float _fishHealth = 0f;

        public event Action OnFishHealthChanged;
        public event Action<int> OnDamage;

        public float FullFishHealth => _fullFishHealth;
        public float Damage => _damage;
        public float DamageDelay => _damageDelay;
        public float FishHealth
        {
            get{return _fishHealth;}
            set{
                float newValue = Mathf.Clamp(value, 0, _fullFishHealth);
                if(_fishHealth != newValue)
                {
                    _fishHealth = value;
                    OnFishHealthChanged?.Invoke();
                }
            }
        }

        public void DoDamage(int damage)
        {
            FishHealth -= damage;
            OnDamage?.Invoke(damage);
        }

        public void ResetHealth()
        {
            _fishHealth = Mathf.RoundToInt(UnityEngine.Random.Range(_minFishHealth, _maxFishHealth));
            _fullFishHealth = _fishHealth;
        }
    }
}
