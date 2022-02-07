using Game.Models;
using UnityEngine;
using NaughtyAttributes;
using Game.Data;
using Game.QuestSystem.Data;

namespace Game.QuestSystem.Map.Extra
{
    public class QuestItem : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private bool _useCustomeData;

        [ShowIf("_useCustomeData")]
        [SerializeField] private QuestItemData _customeData;

        #pragma warning restore 0649
        #endregion
        private SpesialMessagesModel SpesialMessagesModel => ModelsSystem.Instance._spesialMessagesModel;
        private QuestsModel QuestsModel => ModelsSystem.Instance._questsModel;
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;

        public void ReciveQuestItem()
        {
            RecivedItemMessageData data;
            QuestItemData targetItem;
            if(_useCustomeData)
            {
                targetItem = _customeData;
            }
            else
            {
                targetItem = QuestsModel.QuestItemData;
            }
            data = new RecivedItemMessageData(targetItem.ItemIcon,
                                              LocalizationModel.GetString(targetItem.LocalizationKeyID),
                                              string.Empty);

            SpesialMessagesModel.RecivedItem(data);
        }
    }
}