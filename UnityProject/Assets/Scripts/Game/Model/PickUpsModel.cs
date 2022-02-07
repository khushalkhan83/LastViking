using System;
using UnityEngine;

namespace Game.Models
{
    public class PickUpsModel : MonoBehaviour
    {
        public event Action<string, int> OnPickUp;

        public void PickUp(string itemName, int count) => OnPickUp?.Invoke(itemName, count);
    }
}
