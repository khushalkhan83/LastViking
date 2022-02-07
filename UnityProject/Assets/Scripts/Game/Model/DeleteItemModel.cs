using System;
using UnityEngine;

namespace Game.Models
{
    public class DeleteItemModel : MonoBehaviour
    {
        public event Action<ItemsContainer, int> OnShowDeletePopup;

        public void ShowDeletePopup(ItemsContainer container, int cellId)
        {
            OnShowDeletePopup?.Invoke(container, cellId);
        }
    }
}
