using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class InfoPanelDefenceViewModel : MonoBehaviour
    {
        public LocalizationKeyID TitleTextID { get; private set; }
        public LocalizationKeyID DescriptionTextID { get; private set; }
        public ItemData ItemData { get; private set; }

        public void SetTitle(LocalizationKeyID titleTextID) => TitleTextID = titleTextID;
        public void SetDescription(LocalizationKeyID descriptionTextID) => DescriptionTextID = descriptionTextID;
        public void SetHealthItem(ItemData health) => ItemData = health;
    }
}
