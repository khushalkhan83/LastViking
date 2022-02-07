using Game.QuestSystem.Data;
using UnityEngine;

namespace Game.Models
{
    public class QuestStageDesciptionModel : MonoBehaviour
    {
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;
        public string GetDescription(StageData data)
        {
            if(data.useLocID)
                return LocalizationModel.GetString(data.descriptionKey);
            else
            {
                return data.description;
            }
        }
    }
}
