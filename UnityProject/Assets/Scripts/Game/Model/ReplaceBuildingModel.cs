using System;
using EasyBuildSystem.Runtimes.Internal.Part;
using UltimateSurvival.Building;
using UnityEngine;

namespace Game.Models
{
    public class ReplaceBuildingModel : MonoBehaviour
    {
        public event Action OnReplaceBuilding;
        public event Action OnReplaceConstruction;
        public event Action OnPlaceReplacedBuilding;
        public event Action OnPlaceReplacedConstruction;
        public event Action OnReplaceBuildingActiveChanged;
        public event Action OnReplaceConstructionActiveChanged;

        public BuildingPiece Building {get; private set;}
        public PartBehaviour ConstructionpPartBehaviour {get; private set;}
        public bool BuildingReplaceActive {get; private set;}
        public bool ConstructionReplaceActive {get; private set;}

        public void ReplaceBuilding(BuildingPiece building)
        {
            if(building != null)
            {
                Building = building;
                OnReplaceBuilding?.Invoke();
            }
        }

        public void ReplaceConstruction(PartBehaviour partBehaviour)
        {
            if(partBehaviour != null)
            {
                ConstructionpPartBehaviour = partBehaviour;
                OnReplaceConstruction?.Invoke();
            }
        }

        public void PlaceReplacedBuilding()
        {
            if(BuildingReplaceActive)
                OnPlaceReplacedBuilding?.Invoke();
            else if(ConstructionReplaceActive)
                OnPlaceReplacedConstruction?.Invoke();
        }

        public void SetReplaceBuildingActive(bool isActive)
        {
            if(BuildingReplaceActive != isActive)
            {
                BuildingReplaceActive = isActive;
                OnReplaceBuildingActiveChanged?.Invoke();
            }
        }

        public void SetReplaceConstructionActive(bool isActive)
        {
            if(ConstructionReplaceActive != isActive)
            {
                ConstructionReplaceActive = isActive;
                OnReplaceConstructionActiveChanged?.Invoke();
            }
        }


    }
}
