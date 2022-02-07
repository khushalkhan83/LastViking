using System;
using UnityEngine;

namespace Game.Models
{
    public class ItemsOwnershipModel : MonoBehaviour
    {
        public event Action<int, int> OnItemOwnedByPlayer;

        public void ItemOwnedByPlayer(int itemId, int count) => OnItemOwnedByPlayer?.Invoke(itemId, count);
    }
}
