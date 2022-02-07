using System.Linq;
using System.Text;
using NaughtyAttributes;
using UnityEngine;

namespace UltimateSurvival
{
    public class ItemDatabase : ScriptableObject
    {
        public ItemCategory[] Categories { get { return m_Categories; } }

        #region Data
#pragma warning disable 0649

        [SerializeField] private ItemCategory[] m_Categories;
        [SerializeField] private ItemProperty.Definition[] m_ItemProperties;

#pragma warning restore 0649
        #endregion

#if UNITY_EDITOR
        [SerializeField] private int _itemIdCurrent;
#endif

        public ItemData GetItemById(int id) => Categories.SelectMany(x => x.Items).First(x => x.Id == id);
        public bool TryGetItemById(int id, out ItemData item)
        {
            item = Categories.SelectMany(x => x.Items).FirstOrDefault(x => x.Id == id);
            return item != null;
        }

        public ItemData GetItemByName(string name) => Categories.SelectMany(x => x.Items).First(x => x.Name == name);

        public bool FindItemByName(string name, out ItemData itemData) => (itemData = Categories.SelectMany(x => x.Items).FirstOrDefault(x => x.Name == name)) != null;
        
        [Space]
        [SerializeField] private string propertoSearch = "IsEpic";
        
        [Button] void DebugItemToConsoleByPropName()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var cat in m_Categories)
            {
                foreach (var item in cat.Items)
                {
                    if (item.IsHasProperty(propertoSearch))
                        sb.AppendLine(item.Name);
                }
            }

            Debug.Log(sb);
        }
    }
}
