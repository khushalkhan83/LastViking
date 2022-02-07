using System;
using System.Linq;
using Core;
using Core.Controllers;
using Game.Data.Locations;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerLocationController : IPlayerLocationController, IController
    {
        [Inject] public SECTR_Member PlayerSectrMember {get; private set;}
        [Inject] public PlayerLocationModel Model {get; private set;}
        [Inject] public PlayerScenesModel PlayerScenesModel {get; private set;}

        private const string chunkIndexPrefixWord = "Area";

        void IController.Enable() 
        {
            Model.OnGetLocationData += GetLocationData;
        }
        void IController.Start() { }
        void IController.Disable() 
        {
            Model.OnGetLocationData -= GetLocationData;
        }

        private LocationData GetLocationData()
        {
            LocationId locationId;

            try
            {
                switch (PlayerScenesModel.ActiveEnvironmentSceneID)
                {
                    case EnvironmentSceneID.MainIsland:
                        if(PlayerScenesModel.SceneLoading)
                            locationId = LocationId.Unknown;
                        else
                            locationId = GetMainIslandLocationId();
                        break;
                    case EnvironmentSceneID.Waterfall:
                        locationId = LocationId.Waterfall;
                        break;
                    case EnvironmentSceneID.Church:
                        locationId = LocationId.Church;
                        break;
                    default:
                        locationId = LocationId.Unknown;
                        break;
                }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e.ToString());
                locationId = LocationId.Unknown;
            }

            return new LocationData(locationId);
        }

        private LocationId GetMainIslandLocationId()
        {
            LocationId answer = LocationId.Unknown;

            var sector = GetCurrentSector();
            if (sector == null) return answer;
            int areaIndex = GetAreaIndex();

            if (Enum.IsDefined(typeof(LocationId), areaIndex))
                answer = ((LocationId)areaIndex);

            return answer;

            int GetAreaIndex()
            {
                var words = sector.name.Split('_').ToList();

                var indexPrefixWord = words.IndexOf(chunkIndexPrefixWord);

                var targetWord = words[indexPrefixWord + 1];
                return Int32.Parse(targetWord);
            }
        }

        private SECTR_Sector GetCurrentSector()
        {
            PlayerSectrMember.enabled = false;
            PlayerSectrMember.enabled = true;
            var sector = PlayerSectrMember.Sectors.FirstOrDefault();

            return sector;
        }
    }
}