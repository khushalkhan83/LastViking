using Game.Models;
using Game.Spawners.AutoRespawn;
using UltimateSurvival;
using UnityEngine;

namespace Game.OpenWorld.Loot
{
    public class LootCreator : WorldObjectCreatorBase
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private string _itemName;
        [SerializeField] private int _count = 1;
        #pragma warning restore 0649
        #endregion

        #region Dependencies
        private WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        #endregion

        public override bool TryCreateInstance(Vector3 position, Quaternion rotation, Vector3 localScale,DataProcessing dataProcessing, Transform transform, out WorldObjectModel answer)
        {
            answer = null;
            var itemData = ItemsDB.GetItem(_itemName);

            if(itemData != null)
            {
                var instance = WorldObjectCreator.CreateAsSpawnable(WorldObjectID.bag_pickup_floating, position, rotation, localScale, dataProcessing, transform);

                ItemPickup itemPickup = instance.GetComponentInChildren<ItemPickup>();
                itemPickup.SetItemToAdd(new SavableItem(ItemsDB.GetItem(_itemName),_count,null));

                answer = instance;
                return true;

            }
            else return false;
        }
    }
}