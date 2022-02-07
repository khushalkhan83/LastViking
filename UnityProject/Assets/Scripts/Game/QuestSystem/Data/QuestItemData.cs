using Game.Models;
using RoboRyanTron.SearchableEnum;
using UnityEngine;
using NaughtyAttributes;

namespace Game.QuestSystem.Data
{
    [CreateAssetMenu(fileName = "SO_Quest_Item_new", menuName = "Quests/QuestItemData", order = 0)]
    public class QuestItemData : ScriptableObject
    {
        #region Data
        #pragma warning disable 0649
        [ShowAssetPreview]
        [SerializeField] private Sprite itemIcon;
        [SerializeField] private string itemName;
        [SearchableEnum]
        [SerializeField] private LocalizationKeyID localizationKeyID;
        #pragma warning restore 0649
        #endregion

        public Sprite ItemIcon => itemIcon;
        public string ItemName => itemName;
        public LocalizationKeyID LocalizationKeyID => localizationKeyID;
    }
}
