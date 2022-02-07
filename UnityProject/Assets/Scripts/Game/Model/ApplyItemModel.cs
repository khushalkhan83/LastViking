using UnityEngine;
using System;

namespace Game.Models
{
    public class ApplyItemModel : MonoBehaviour
    {
        public event Action<ItemsContainer, CellModel> OnPreApplyItem;
        public event Action<ItemsContainer, CellModel> OnApplyItem;

        public void ApplyItem(ItemsContainer container, CellModel cell)
        {
            OnPreApplyItem?.Invoke(container, cell);
            OnApplyItem?.Invoke(container, cell);
        }
    }
}
