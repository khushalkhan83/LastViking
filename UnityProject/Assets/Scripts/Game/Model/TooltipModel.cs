using System;
using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.Models
{
    public class TooltipModel : MonoBehaviour
    {
        private const string tooltipPrefix = "_tooltip";

        #region Dependencies
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;
        #endregion        

        public bool TryGetLocalizedItemTooltipText(string itemName, out string answer)
        {
            var localizationKeyText = itemName + tooltipPrefix;

            if(Enum.TryParse(localizationKeyText, out LocalizationKeyID key))
            {
                answer = LocalizationModel.GetString(key);
                return true;
            }
            else
            {
                answer = string.Empty;
                return false;
            }
        }

        public bool TryGetLocalizedQuestItemTooltipText(QuestItemData questItemData, out string answer)
        {
            var localizationKeyText = questItemData.LocalizationKeyID + tooltipPrefix;

            if(Enum.TryParse(localizationKeyText, out LocalizationKeyID key))
            {
                answer = LocalizationModel.GetString(key);
                return true;
            }
            else
            {
                answer = string.Empty;
                return false;
            }
        }
    }
}