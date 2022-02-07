using Game.AI;
using UnityEngine;
using UnityEngine.Events;

namespace Game.Models
{
    public class WorldObjectDamageReceiver : MonoBehaviour, IDamageable
    {
#pragma warning disable 0649

        [SerializeField] private WorldObjectModel _worldObjectModel;
        [SerializeField] private UnityEvent _onDamageEvent;

#pragma warning restore 0649

        private WorldObjectModel WorldObjectModel => _worldObjectModel;

        private IHealth _health;
        private IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());
        private StatisticWorldObjectsNodel StatisticWorldObjectsNodel => ModelsSystem.Instance._statisticWorldObjectsNodel;

        void IDamageable.Damage(float value, GameObject from)
        {
            Health.AdjustHealth(-value);

            if (Health.IsDead)
            {
                StatisticWorldObjectsNodel.Kill(from?.GetComponent<Target>()?.ID ?? TargetID.None, WorldObjectModel.WorldObjectID);
            }
            _onDamageEvent?.Invoke();
        }
    }
}
