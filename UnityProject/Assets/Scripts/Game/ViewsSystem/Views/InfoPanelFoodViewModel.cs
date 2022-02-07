using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class InfoPanelFoodViewModel : MonoBehaviour
    {
        public LocalizationKeyID TitleTextID { get; private set; }
        public LocalizationKeyID DescriptionTextID { get; private set; }
        public ItemData FoodItem { get; private set; }
        public int HealthValue { get; private set; }
        public int HungerValue { get; private set; }
        public int ThirstValue { get; private set; }

        public void SetTitle(LocalizationKeyID titleTextID) => TitleTextID = titleTextID;
        public void SetDescription(LocalizationKeyID descriptionTextID) => DescriptionTextID = descriptionTextID;
        public void SetFoodItem(ItemData foodItem) => FoodItem = foodItem;
        public void SetHealth(int health) => HealthValue = health;
        public void SetHunger(int hunger) => HungerValue = hunger;
        public void SetThirst(int thirst) => ThirstValue = thirst;
    }
}