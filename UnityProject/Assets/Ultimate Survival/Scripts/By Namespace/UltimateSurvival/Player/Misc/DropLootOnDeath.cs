using Game.Models;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class DropLootOnDeath : EntityBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private CellSpawnSettings[] _cellsSettings;

#pragma warning restore 0649
        #endregion

        public IEnumerable<CellSpawnSettings> CellsSettings => _cellsSettings;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

        private void OnEnable()
        {
            Entity.Death.AddListener(OnDeathHandler);
        }

        private void OnDeathHandler()
        {
            foreach (var cellSettings in CellsSettings)
            {
                var item = cellSettings.GenerateItem(ItemsDB.ItemDatabase);
                var itemPickup = WorldObjectCreator.Create(item.ItemData.WorldObjectID, transform.position, Quaternion.identity).GetComponentInChildren<ItemPickup>();
                itemPickup.SetItemToAdd(item);
            }
        }
    }
}
