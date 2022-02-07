using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UltimateSurvival.Debugging;

namespace CustomeEditorTools
{
    [CreateAssetMenu(fileName = "SO_itemsGroup_new", menuName = "InventoryRelated/ItemsGroup", order = 0)]
    public class ItemsGroup : ScriptableObject
    {
        [SerializeField] private List<ItemsList> itemsLists;
        public List<ItemMeta> Items => itemsLists.SelectMany(x => x.Items).ToList();
        public List<ItemsList> ItemsLists => itemsLists;
    }
}
