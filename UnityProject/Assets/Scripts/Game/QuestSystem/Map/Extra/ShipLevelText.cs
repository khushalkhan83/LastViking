using System.Collections;
using System.Collections.Generic;
using Game.Models;
using UnityEngine;
using UnityEngine.UI;

namespace Game.QuestSystem.Map.Extra
{
    public class ShipLevelText : MonoBehaviour
    {
        [SerializeField] private Text shelterLevelText;

        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
        private LocalizationModel LocalizationModel => ModelsSystem.Instance._localizationModel;

        private void Start()
        {
            int shipLevel = 0;
            if (SheltersModel.ShelterActive == ShelterModelID.None || SheltersModel.ShelterActive == ShelterModelID.Ship)
            {
                shipLevel = SheltersModel.ShelterModel.Level;// + 1;
            }

            shelterLevelText.text = LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_Level) + " " + shipLevel;    
        }
    }
}
