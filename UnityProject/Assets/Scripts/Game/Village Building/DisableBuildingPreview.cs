using System.Collections;
using System.Collections.Generic;
using EasyBuildSystem.Runtimes.Internal.Part;
using Game.Models;
using UnityEngine;

namespace Game.VillageBuilding
{
    public class DisableBuildingPreview : MonoBehaviour
    {
        [SerializeField] private GameObject[] buildingPreviews;
        [SerializeField] private PartBehaviour partBehaviour;

        private BuildingModeModel BuildingModeModel => ModelsSystem.Instance._buildingModel;

        private void OnEnable() 
        {
            BuildingModeModel.BuildingEnabled += UpdatePreviewVisisble;
            BuildingModeModel.BuildingDisabled += UpdatePreviewVisisble;
            UpdatePreviewVisisble();
        }

        private void OnDisable() 
        {
            BuildingModeModel.BuildingEnabled -= UpdatePreviewVisisble;
            BuildingModeModel.BuildingDisabled -= UpdatePreviewVisisble;
            foreach(var preview in buildingPreviews)
            {
                preview.SetActive(true);
            }
        }

        private void UpdatePreviewVisisble()
        {
            foreach(var preview in buildingPreviews)
            {
                if(partBehaviour.CurrentState == StateType.Placed)
                    preview.SetActive(BuildingModeModel.BuildingActive);
                else
                    preview.SetActive(true);
            }
        }
        
    }
}
