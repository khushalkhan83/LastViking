using System;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public class VillageCinematicsModel : MonoBehaviour
    {
        public event Action OnHallCinematicStart;

        [Button]   
        public void HallCinematicStart()
        {
            OnHallCinematicStart?.Invoke();
        }
    }
}
