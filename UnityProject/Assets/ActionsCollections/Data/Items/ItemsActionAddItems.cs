using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class ItemsActionAddItems : ActionBase
    {
        private ItemsContainer ItemsContainer => ModelsSystem.Instance._inventoryModel.ItemsContainer;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private string _operationName;
        private readonly List<ItemMeta> _items;

        public ItemsActionAddItems(string name, List<ItemMeta> items)
        {
            _operationName = name;
            _items = items;
        }
        public override string OperationName => _operationName;

        //TODO: remove Dublicate logic in InventoryModel
        public override void DoAction()
        {
            foreach (var item in _items)
            {
                ItemsContainer.AddItems(ItemsDB.GetItem(item.Name).Id, item.Count);
            }
        }
    }
}