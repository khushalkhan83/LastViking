using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

namespace Game.AI.BehaviorDesigner
{
    public class EnemyDamageListener : MonoBehaviour
    {
        [SerializeField] private EnemyDamageReciver enemy;

        #pragma warning disable 0649
        [SerializeField] private UnityEvent onRecivedDamage;
        #pragma warning restore 0649


        #region MonoBehaviour
        private void OnEnable()
        {
            enemy.OnReceiveDamage += OnReceiveDamage;
        }

        private void OnDisable()
        {
            enemy.OnReceiveDamage -= OnReceiveDamage;

        }
        #endregion

        #if UNITY_EDITOR
        [Button]
        public void Test() => Received();
        #endif

        private void OnReceiveDamage(EnemyDamageReciver obj)
        {
            Received();
        }

        private void Received() => onRecivedDamage?.Invoke();
    }
}