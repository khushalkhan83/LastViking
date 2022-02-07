using Game.Providers;
using System;
using UnityEngine;

namespace Game.Models
{
    public class EventTokensModel : MonoBehaviour
    {
        #region Data
        [SerializeField] private string _tombPrefix = "Tomb";
        [SerializeField] private string _shelterPrefix = "Shelter";

        [SerializeField] private SpriteID _tombIconId = SpriteID.TokenX; // not used
        [SerializeField] private SpriteID _shelterIconId = SpriteID.TokenShelter; // not used

        [SerializeField] private WorldObjectID _tombId = WorldObjectID.Tomb;
        [SerializeField] private ShelterModelID _shelterId = ShelterModelID.Ship;
        #endregion

        public string TombPrefix => _tombPrefix;
        public string ShelterPrefix => _shelterPrefix;

        public WorldObjectID TombId => _tombId;
        public ShelterModelID ShelterId => _shelterId;

        public SpriteID TombIconId => _tombIconId;
        public SpriteID ShelterIconId => _shelterIconId;
    }
}
