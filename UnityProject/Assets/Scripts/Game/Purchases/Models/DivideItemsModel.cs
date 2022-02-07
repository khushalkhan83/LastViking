using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class DivideItemsModel : MonoBehaviour
    {
        public event Action<ItemsContainer, CellModel> OnShowDividePopup;
        public event Action<SavableItem> OnItemSplitted;
        public event Action OnHideDividePopup;

        /* Refactor logic: dont call Hide() explicitly, but provied this to cliend */

        public void ShowDividePopup(ItemsContainer container, CellModel cell) => OnShowDividePopup?.Invoke(container, cell);
        public void ItemSplitted(SavableItem item) => OnItemSplitted?.Invoke(item);
        public void HideDividePopup() => OnHideDividePopup?.Invoke();
    }
}
