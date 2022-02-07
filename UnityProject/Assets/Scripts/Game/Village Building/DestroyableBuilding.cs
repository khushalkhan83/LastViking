using System;
using System.Collections;
using System.Collections.Generic;
using Game.AI;
using Game.Models;
using NaughtyAttributes;
using UltimateSurvival;
using UnityEngine;

namespace Game.VillageBuilding
{
    public class DestroyableBuilding : MonoBehaviour
    {
        [SerializeField] private GameObject normalBuildingView = default;
        [SerializeField] private GameObject destroyedBuildingView = default;
        [SerializeField] private BuildingHealthModel buildingHealthModel = default;
        [SerializeField] private HouseBuilding houseBuilding = default;
        [SerializeField] private Target target = default;

        public BuildingHealthModel BuildingHealthModel => buildingHealthModel;
        public Target Target => target;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private VillageBuildingModel VillageBuildingModel => ModelsSystem.Instance._villageBuildingModel;

        private void OnEnable() 
        {
            buildingHealthModel.OnChangeHealth += OnChangeHealth;
            UpdateBuildingView();
        }

        private void OnDisable() 
        {
            buildingHealthModel.OnChangeHealth -= OnChangeHealth;
        }

        private void OnChangeHealth()
        {
            UpdateBuildingView();
        }

        private void UpdateBuildingView()
        {
            bool isDestroyed = buildingHealthModel.IsDead;
            normalBuildingView.SetActive(!isDestroyed);
            destroyedBuildingView.SetActive(isDestroyed);
        }

        [Button]
        public void RestoreBuilding()
        {
            buildingHealthModel.SetHealth(buildingHealthModel.HealthMax);
        }

        public List<RequiredItem> GetRestoreItems()
        {
            float restoreHealth = Mathf.Clamp(buildingHealthModel.HealthMax - buildingHealthModel.Health, 0, buildingHealthModel.HealthMax);
            return VillageBuildingModel.GetRestoreItems(houseBuilding.Type, houseBuilding.Level, restoreHealth, buildingHealthModel.HealthMax);
        }

    }
}
