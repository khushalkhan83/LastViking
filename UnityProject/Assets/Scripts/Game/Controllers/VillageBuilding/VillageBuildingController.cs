using Core;
using Core.Controllers;
using Game.Models;
using Game.VillageBuilding;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class VillageBuildingController : IVillageBuildingController, IController
    {
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        private bool IgnoreItemsPrice => EditorGameSettings.Instance.IgnoreItemsPrice;
        
        void IController.Enable() 
        {
            VillageBuildingModel.OnCanUpgradeBuilding += CanUpgradeBuilding;
            VillageBuildingModel.OnIsUpgradeUnlocked += OnIsUpgradeUnlocked;
            VillageBuildingModel.OnGetRestoreItems += OnGetRestoreItems;
            VillageBuildingModel.OnActiveBuildingsChanged += UpdateCitizensCount;
            VillageBuildingModel.OnHouseStateChanged += UpdateCitizensCount;
            UpdateCitizensCount();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            VillageBuildingModel.OnCanUpgradeBuilding -= CanUpgradeBuilding;
            VillageBuildingModel.OnIsUpgradeUnlocked -= OnIsUpgradeUnlocked;
            VillageBuildingModel.OnGetRestoreItems -= OnGetRestoreItems;
            VillageBuildingModel.OnActiveBuildingsChanged -= UpdateCitizensCount;
            VillageBuildingModel.OnHouseStateChanged -= UpdateCitizensCount;
        }

        private bool CanUpgradeBuilding(HouseBuilding building)
        {
            int nextLevel = building.Level + 1;
            var info = VillageBuildingModel.GetHouseBuildingInfo(building.Type);

            if(nextLevel > info.MaxLevel)
                return false;

            if(!TutorialModel.IsComplete && !VillageBuildingModel.BuildingsAvailableInTutorial.Contains(building.Type))
                return false;

            var levelInfo = info.GetLevelInfo(nextLevel);

            
            if(VillageBuildingModel.FreeCitizensCount < levelInfo.requiredCitizens)
                return false;

            if(!IgnoreItemsPrice && !InventoryOperationsModel.PlayerHasItems(levelInfo.requiredItems))
                return false;

            foreach(var requiredBuilding in levelInfo.requiredBuildings)
            {
                if(!HasBuilding(requiredBuilding))
                    return false;
            }

            return true;
        }

        private bool OnIsUpgradeUnlocked(HouseBuilding building)
        {
            int nextLevel = building.Level + 1;
            var info = VillageBuildingModel.GetHouseBuildingInfo(building.Type);

            if(nextLevel > info.MaxLevel)
                return false;

            if(!TutorialModel.IsComplete && !VillageBuildingModel.BuildingsAvailableInTutorial.Contains(building.Type))
                return false;

            var levelInfo = info.GetLevelInfo(nextLevel);
            
            if(VillageBuildingModel.FreeCitizensCount < levelInfo.requiredCitizens)
                return false;

            foreach(var requiredBuilding in levelInfo.requiredBuildings)
            {
                if(!HasBuilding(requiredBuilding))
                    return false;
            }

            return true;
        }

        public List<RequiredItem> OnGetRestoreItems(HouseBuildingType type, int level, float restoreHealth, float fullHealth)
        {
            var info = VillageBuildingModel.GetHouseBuildingInfo(type);
            var levelInfo = info.GetLevelInfo(level);
            List<RequiredItem> restoreItems = new List<RequiredItem>();
            foreach(var item in levelInfo.requiredItems)
            {
                float correlation = (restoreHealth / fullHealth) * VillageBuildingModel.RestoreBuildingResourcesCorrelation;
                var restoreItem = new RequiredItem(item.Name, Mathf.CeilToInt(item.Amount * correlation));
                restoreItems.Add(restoreItem);
            }
            return restoreItems;
        }

        private bool HasBuilding(RequiredBuilding requiredBuilding)
        {
            return VillageBuildingModel.ActiveBuildings.Where(b => b.Type == requiredBuilding.type && b.Level >= requiredBuilding.level).Count() >= requiredBuilding.count;
        }

        private void UpdateCitizensCount()
        {
            int allCitizens = 0;
            int busyCitizens = 0;
            foreach(var building in VillageBuildingModel.ActiveBuildings)
            {
                if(building.Level > 0)
                {
                    var info = VillageBuildingModel.GetHouseBuildingInfo(building.Type);
                    var levelInfo = info.GetLevelInfo(building.Level);
                    if(levelInfo != null)
                    {
                        allCitizens += levelInfo.addsCitizens;
                        busyCitizens += levelInfo.requiredCitizens;
                    }
                }
            }
            VillageBuildingModel.SetCitizensCount(allCitizens, busyCitizens);
        }

    }
}
