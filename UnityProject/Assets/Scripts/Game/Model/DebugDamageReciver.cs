using UnityEngine;

namespace Game.Models
{
    public class DebugDamageReciver : MonoBehaviour, IDamageable
    {
        [SerializeField] private DebugDamageDisplay debugDamageDisplay;
        [SerializeField] private Transform rootObject;
        void IDamageable.Damage(float value, GameObject from)
        {
            rootObject.LookAt(from.transform);
            debugDamageDisplay.SetData(value.ToString());
        }
    }
}
