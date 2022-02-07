using System;
using UnityEngine;

namespace Game.Models
{
    public class ConstructionModel : MonoBehaviour
    {
        public event Action<string> OnPlacePart;
        
        public void PlacePart(string partId)
        {
            OnPlacePart?.Invoke(partId);
        }

    }
}
