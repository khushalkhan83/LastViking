using System;
using Game.Data.Locations;
using UnityEngine;

namespace Game.Models
{
    public class AnalyticsPlayerLocationModel : MonoBehaviour
    {
        [SerializeField] private float sendEventIntervalTime = 180f;

        public event Action<LocationId> OnSendPlayerLocationEvent;

        public float SendEventIntervalTime => sendEventIntervalTime;
        
        public void SendEvent(LocationId locationId) => OnSendPlayerLocationEvent?.Invoke(locationId);
    }
}
