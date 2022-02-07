using System;
using UnityEngine;

namespace Game.Models
{
    public class PlacementModel : MonoBehaviour
    {
        public event Action<int> OnPlaceItem;

        public void PlaceItem(int itemId) => OnPlaceItem?.Invoke(itemId);
    }
}
