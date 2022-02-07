using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class InfoPanelWeaponViewModel : MonoBehaviour
    {
        public LocalizationKeyID TitleTextID { get; private set; }
        public LocalizationKeyID DescriptionTextID { get; private set; }
        public ItemData Weapon { get; private set; }
        public int WeaponDamage { get; private set; }
        public float WeaponDurability { get; private set; }

        public void SetTitle(LocalizationKeyID titleTextID) => TitleTextID = titleTextID;
        public void SetDescription(LocalizationKeyID descriptionTextID) => DescriptionTextID = descriptionTextID;
        public void SetWeapon(ItemData weapon) => Weapon = weapon;
        public void SetDamage(int damage) => WeaponDamage = damage;
        public void SetDurability(float durability) => WeaponDurability = durability;
    }
}
