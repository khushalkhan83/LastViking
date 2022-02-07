using System.Collections.Generic;
using NaughtyAttributes;
using UltimateSurvival.Debugging;
using UnityEngine;
// using UnityEditor;
// using Game.Models;

namespace CustomeEditorTools
{
    [CreateAssetMenu(fileName = "SO_items_new", menuName = "InventoryRelated/ItemsList", order = 0)]
    public class ItemsList : ScriptableObject
    {
        [SerializeField] [ReorderableList] private List<ItemMeta> items;
        public List<ItemMeta> Items => items;
    }
}
