using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class DropItemModel : MonoBehaviour
    {
        public event Func<SavableItem,Transform,bool, GameObject> OnItemDrop;
        public event Func<SavableItem,Vector3, bool, bool, GameObject> OnItemDropFloating;
        public event Action<GameObject> OnItemDropped;
        public event Action<GameObject> OnItemDroppedFloating;

        public GameObject DropItem(SavableItem item,Transform dropPosition = null,bool removeAutoDestroyComponent = false) => OnItemDrop?.Invoke(item,dropPosition,removeAutoDestroyComponent);

        public GameObject DropItemFloating(SavableItem item, Vector3 position, bool autopickup = false, bool closeToPlayer = false) => OnItemDropFloating?.Invoke(item, position, autopickup, closeToPlayer);

        public void ItemDropped(GameObject item) => OnItemDropped?.Invoke(item);
        public void ItemDroppedFloating(GameObject item) => OnItemDroppedFloating?.Invoke(item);
    }
}
