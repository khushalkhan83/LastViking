using Game.AI.BehaviorDesigner;
using NaughtyAttributes;
using UnityEngine;

namespace Game.FX.Test
{
    public class EnemyDamageListenerTest : MonoBehaviour
    {
        private EnemyDamageListener[] listeners;

        private void Awake()
        {
            listeners = GetComponentsInChildren<EnemyDamageListener>();
        }

        #if UNITY_EDITOR
        [Button] void TestAll()
        {
            foreach (var enemy in listeners)
            {
                enemy.Test();
            }
        }
        #endif
    }
}