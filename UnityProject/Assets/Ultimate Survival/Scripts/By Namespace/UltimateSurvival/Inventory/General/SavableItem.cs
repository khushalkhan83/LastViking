using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class SavableItem
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _id;
        [SerializeField] private int m_CurrentInStack;
        [SerializeField] private List<ItemProperty.Value> m_CurrentPropertyValues;

#pragma warning restore 0649
        #endregion

        public Message<ItemProperty.Value> PropertyChanged = new Message<ItemProperty.Value>();
        public Message StackChanged = new Message();

        public ItemData ItemData { get; set; }

        public List<ItemProperty.Value> CurrentPropertyValues => m_CurrentPropertyValues;

        public int Id => _id;
        public string Name => ItemData.Name;

        public int Count
        {
            get => m_CurrentInStack;
            set
            {
                m_CurrentInStack = value;
                StackChanged.Send();
            }
        }

        public bool IsFullStack => Count == ItemData.StackSize;
        public bool IsCanStackable => ItemData.StackSize > 1;

        public SavableItem() { } // Need for unity serialization

        public SavableItem(ItemData data, int currentInStack = 1, List<ItemProperty.Value> customPropertyValues = null)
        {
            Count = Mathf.Clamp(currentInStack, 1, data.StackSize);

            if (customPropertyValues != null)
            {
                m_CurrentPropertyValues = customPropertyValues.Select(x => x.Clone()).ToList();
            }
            else
            {
                m_CurrentPropertyValues = data.PropertyValues.Select(x => x.Clone()).ToList();
            }

            _id = data.Id;
            ItemData = data;

            for (int i = 0; i < m_CurrentPropertyValues.Count; i++)
            {
                m_CurrentPropertyValues[i].Changed.AddListener(On_PropertyChanged);
            }
        }

        public SavableItem(SavableItem item)
        {
            ItemData = item.ItemData;
            m_CurrentInStack = item.Count;
            _id = item.Id;
            m_CurrentPropertyValues = item.m_CurrentPropertyValues.Select(x => x.Clone()).ToList();
        }

        public bool HasProperty(string name) => m_CurrentPropertyValues.Any(x => x.Name == name);

        public ItemProperty.Value GetProperty(string name) => m_CurrentPropertyValues.FirstOrDefault(x => x.Name == name);

        public bool TryGetProperty(string name, out ItemProperty.Value propertyValue) => (propertyValue = GetProperty(name)) != null;

        public void SetProperty(ItemProperty.Value propertyValue) => On_PropertyChanged(propertyValue);

        public int AdjustCount(int count)
        {
            var leftCount = Count + count;

            Count = Math.Min(Math.Max(Count + count, 0), ItemData.StackSize);

            if (leftCount < 0)
            {
                return leftCount;
            }

            if (leftCount > ItemData.StackSize)
            {
                return leftCount - ItemData.StackSize;
            }

            return 0;
        }

        private void On_PropertyChanged(ItemProperty.Value propertyValue) => PropertyChanged.Send(propertyValue);
    }
}
