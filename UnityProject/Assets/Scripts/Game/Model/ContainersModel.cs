using System.Collections;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class ContainersModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] InventoryModel _inventoryModel;
        [SerializeField] HotBarModel _hotBarModel;
        [SerializeField] LoomGroupModel _loomGroupModel;
        [SerializeField] LootGroupModel _lootGroupModel;
        [SerializeField] CampFiresModel _campFiresModel;
        [SerializeField] FurnaceGroupModel _furnaceGroupModel;
        [SerializeField] InventoryEquipmentModel _inventoryEquipmentModel;

#pragma warning restore 0649
        #endregion

        public ItemsContainer GetContainer(ContainerID containerID) 
        {
            switch (containerID)
            {
                case ContainerID.None:
                    return null;
                case ContainerID.Inventory:
                    return _inventoryModel.ItemsContainer;
                case ContainerID.HotBar:
                    return _hotBarModel.ItemsContainer;
                case ContainerID.Loom:
                    return _loomGroupModel.ActiveLoom.ItemsContainer;
                case ContainerID.Loot:
                    return _lootGroupModel.ActiveLoot.ItemsContainer;
                case ContainerID.Furnace:
                    return _furnaceGroupModel.ActiveFurnace.ItemsContainer;
                case ContainerID.CampFire:
                    return _campFiresModel.ActiveCampFire.ItemsContainer;
                case ContainerID.Equipment:
                    return _inventoryEquipmentModel.ItemsContainer;
                default:
                    return null;
            }
        }

        public ContainerID GetContainerID(ItemsContainer container)
        {
            if (container == _inventoryModel.ItemsContainer)
                return ContainerID.Inventory;
            if (container == _hotBarModel.ItemsContainer)
                return ContainerID.HotBar;
            if (container == _inventoryEquipmentModel.ItemsContainer)
                return ContainerID.Equipment;
            if (_loomGroupModel.ActiveLoom != null && container == _loomGroupModel.ActiveLoom.ItemsContainer)
                return ContainerID.Loom;
            if (_lootGroupModel.ActiveLoot != null && container == _lootGroupModel.ActiveLoot.ItemsContainer)
                return ContainerID.Loot;
            if (_furnaceGroupModel.ActiveFurnace != null && container == _furnaceGroupModel.ActiveFurnace.ItemsContainer)
                return ContainerID.Furnace;
            if (_campFiresModel.ActiveCampFire != null && container == _campFiresModel.ActiveCampFire.ItemsContainer)
                return ContainerID.CampFire;

            return ContainerID.None;
        }
    }
}
