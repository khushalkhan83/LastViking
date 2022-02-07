using System;
using Game.Data.Locations;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Data.Locations
{
    [Serializable]
    public class LocationData
    {
        public readonly LocationId locationId;
        public LocationData(LocationId locationId)
        {
            this.locationId = locationId;
        }
    }
    public enum LocationId
    {
        Unknown,
        // MainIsland indexes 1 - 20
        MainIsland_01_Start = 1,
        MainIsland_02_Waterfall_Exit = 2,
        MainIsland_03_Sand = 3,
        MainIsland_04_Grand_Road = 4,
        MainIsland_05_Old_Castle = 5,
        MainIsland_06_Old_Town = 6,
        MainIsland_07_Fish_Bay = 7,
        MainIsland_08_End_of_Life = 8,
        MainIsland_09_Town_Backyard = 9,
        MainIsland_10_Lost_Island = 10,
        MainIsland_11_Fishing_Vilidge = 11,
        MainIsland_12_Waterfall_Enterence = 12,

        //Waterfall indexes 20 - 30
        Waterfall = 20,

        // Church indexes 30 - 40
        Church = 30
    }
}

namespace Game.Models
{
    public class PlayerLocationModel : MonoBehaviour
    {
        public event Func<LocationData> OnGetLocationData;

        public LocationData GetLocation()
        {
            if(OnGetLocationData == null)
            {
                "Can`t get location data. Controller not active".Error();
                return new LocationData(LocationId.Unknown);
            }
            else
            {
                return OnGetLocationData.Invoke();
            }
        }

        [Button] void Test_GetLocation()
        {
            Debug.Log($"Location id: {GetLocation().locationId.ToString()}");
        }
    }
}