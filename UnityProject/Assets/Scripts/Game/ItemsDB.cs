using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class ItemsDB : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ItemDatabase _itemDatabase;

#pragma warning restore 0649
        #endregion

        private ItemDatabase __itemDatabase;
        public ItemDatabase ItemDatabase => __itemDatabase ?? (__itemDatabase = Instantiate(_itemDatabase));

        public ItemData GetItem(string name) => ItemDatabase.GetItemByName(name);

        public ItemData GetItem(int id) => ItemDatabase.GetItemById(id);

        public bool TryGetItem(int id, out ItemData item) => ItemDatabase.TryGetItemById(id, out item);
    }
}
