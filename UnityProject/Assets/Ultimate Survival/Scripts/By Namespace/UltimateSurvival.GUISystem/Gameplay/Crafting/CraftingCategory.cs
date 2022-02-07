using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace UltimateSurvival.GUISystem
{
    public class CraftingCategory : Selectable
    {
        /// <summary>Message sent when this category gets selected by the user.</summary>
        public Message<CraftingCategory> Selected = new Message<CraftingCategory>();

        public string DisplayName { get { return m_DisplayName; } }

        #region Data
#pragma warning disable 0649

        [Header("Settings")]

        [SerializeField] private string m_DisplayName = "None";

        [Reorderable]
        [SerializeField] private ReorderableStringList m_CorrespondingCategories;
#pragma warning restore 0649
        #endregion

        public bool HasCategory(string categoryName)
        {
            for (int i = 0; i < m_CorrespondingCategories.Count; i++)
            {
                if (m_CorrespondingCategories[i] == categoryName)
                    return true;
            }

            return false;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            base.OnPointerDown(eventData);
            Selected.Send(this);
        }
    }
}
