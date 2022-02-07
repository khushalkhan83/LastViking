using Extensions;
using UnityEngine;
using Game.Progressables;
using Game.AI.BehaviorDesigner;

namespace Game.Controllers
{
    public class EnemyTriggerSpawner : ObjectsTriggerSpawner<Initable>
    {
        #region Data
#pragma warning disable 0649

        [Header("Settings")]
        [SerializeField] private EnemiesProvider _enemiesProvider;
        [SerializeField] private EnemyID _enemyId;

#pragma warning restore 0649
        #endregion

        public EnemiesProvider EnemiesProvider => _enemiesProvider;
        public EnemyID EnemyID => _enemyId;

        protected override Initable CreateObject() => Instantiate(EnemiesProvider[EnemyID], Container.transform.position, Container.transform.rotation);

        protected override void OnCreate()
        {
            ObjectInstance.transform.parent = Container;
            DestroyController destroyController = ObjectInstance.gameObject.AddComponent<DestroyController>();
            destroyController.OnDestroyAction += OnDestoyEnemy;


            if (ProgressStatus == ProgressStatus.InProgress)
            {
                ActivateObject();
                ObjectInstance.Init(true);
            }
            else
            {
                DeactivateObject();
                ObjectInstance.Init(false);
            }
                
        }

        private void OnDestoyEnemy(DestroyController destroyController)
        {
            destroyController.OnDestroyAction -= OnDestoyEnemy;

            OnDelete();
        }

        protected override void ActivateObject()
        {
            ObjectInstance.TrySetActive(true);
        }

        protected override void DeactivateObject()
        {
            ObjectInstance.TrySetActive(false);
        }

        public override void ClearProgress() { }
    }
}
