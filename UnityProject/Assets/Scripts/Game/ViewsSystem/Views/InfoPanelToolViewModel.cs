using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class InfoPanelToolViewModel : MonoBehaviour
    {
        public LocalizationKeyID TitleTextID { get; private set; }
        public LocalizationKeyID DescriptionTextID { get; private set; }
        public ItemData Tool { get; private set; }
        public int ToolDamage { get; private set; }
        public float ToolDurability { get; private set; }

        public void SetTitle(LocalizationKeyID titleTextID) => TitleTextID = titleTextID;
        public void SetDescription(LocalizationKeyID descriptionTextID) => DescriptionTextID = descriptionTextID;
        public void SetTool(ItemData tool) => Tool = tool;
        public void SetMiningValue(int damage) => ToolDamage = damage;
        public void SetDurability(float durability) => ToolDurability = durability;
    }
}
