using Game.AI.BehaviorDesigner;
using UnityEngine;

namespace SOArchitecture
{
    public class EnemiesGroup: MonoBehaviour
    {
        [SerializeField] private EnemiesContextRuntimeSet set;
        private EnemyContext context;

        private void Awake()
        {
            context = GetComponentInChildren<EnemyContext>();
        }
        public void SetRuntimeSet(EnemiesContextRuntimeSet set)
        {
            RemoveFromSet();
            this.set = set;
            AddToSet();
        }

        private void OnEnable()
        {
            AddToSet();
        }

        private void OnDisable()
        {
            RemoveFromSet();
        }

        private void AddToSet() => set?.Add(context);

        private void RemoveFromSet() => set?.Remove(context);
    }
}