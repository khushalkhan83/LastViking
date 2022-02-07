using System;
using UnityEngine;

namespace Game.Models
{
    public class FixPlayerUndergroundModel : MonoBehaviour
    {
        [SerializeField] private LayerMask _groundLayerMask;
        public LayerMask GroundLayerMask => _groundLayerMask;
    }
}
