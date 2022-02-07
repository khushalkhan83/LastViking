using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class RepairViewModel : MonoBehaviour
    {
        public SavableItem CurrentItem { get; private set; }

        public void SetItem(SavableItem Item) => CurrentItem = Item;
    }
}
