using System;
using UnityEngine;

namespace Game.Models
{
    public class ZoneModel : MonoBehaviour
    {
        public event Action OnPlayerEnterZone;

        public string PlayerZone{get; private set;}
        // public ZoneID PlayerZone{get; private set;}
        public bool PlayerZoneIsSet {get; private set;}

        public void PlayerEnterZone(string zoneID)
        {
            if (PlayerZone != zoneID)
            {
                PlayerZone = zoneID;
                PlayerZoneIsSet = true;
                OnPlayerEnterZone?.Invoke();
            }
        }
    }
}