using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace UltimateSurvival
{
    public class EnemyLootDroper : MonoBehaviour
    {
        [SerializeField] private CellSpawnSettings[] cellsSettings;

        private IHealth _health;
        private IHealth Health => _health ?? (_health = GetComponentInParent<IHealth>());

        public ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        public DropItemModel DropItemModel => ModelsSystem.Instance._dropItemModel;

        private void OnEnable() 
        {
            Health.OnDeath += DropItems;
        }

        private void OnDisable() 
        {
            Health.OnDeath -= DropItems;
        }

        public void DropItems()
        {
            List<SavableItem> items = new List<SavableItem>();
            foreach (var settings in cellsSettings)
            {
                var item = settings.GenerateItem(ItemsDB.ItemDatabase);
                if(item != null)
                {
                    DropItemModel.DropItemFloating(item, transform.position);
                }
            }
            Health.OnDeath -= DropItems;
        }
    }
}
