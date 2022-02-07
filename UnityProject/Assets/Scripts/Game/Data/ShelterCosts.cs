using System;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class ShelterCosts
    {
        [SerializeField] private ShelterCost[] _costs;

        public ShelterCost[] Costs => _costs;
    }
}
