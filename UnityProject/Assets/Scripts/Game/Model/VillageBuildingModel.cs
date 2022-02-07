using System;
using System.Collections.Generic;
using Game.VillageBuilding;
using UltimateSurvival;
using UnityEngine;
using System.Linq;

namespace Game.Models
{
    public class VillageBuildingModel : MonoBehaviour
    {
        [SerializeField] private HouseBuildingInfoDictionary houseBuildingInfoDictionary = default;
        [SerializeField] private List<HouseBuilding> activeBuildings = default;
        [SerializeField] private HouseBuilding townhall = default;
        [Range(0, 1f)]
        [SerializeField] private float restoreBuildingResourcesCorrelation = 0.5f;

        public HouseBuildingInfoDictionary HouseBuildingInfoDictionary => houseBuildingInfoDictionary;
        public List<HouseBuilding> ActiveBuildings => activeBuildings;
        public HouseBuilding Townhall => townhall;
        public float RestoreBuildingResourcesCorrelation => restoreBuildingResourcesCorrelation;
        public int AllCitizensCount {get; private set;}
        public int BusyCitizensCount{get; private set;}
        public HashSet<HouseBuildingType> BuildingsAvailableInTutorial {get; set;} = new HashSet<HouseBuildingType>();
        public int FreeCitizensCount => AllCitizensCount - BusyCitizensCount;

        public event Action OnHouseStateChanged;
        public event Action OnActiveBuildingsChanged;
        public event Action OnCitizensCountChanged;
        public event Action<HouseBuilding> OnStartBuildingProcess;
        public event Action<HouseBuilding> OnCompleteBuildingProcess;
        public event Func<HouseBuilding,bool> OnCanUpgradeBuilding;
        public event Func<HouseBuilding,bool> OnIsUpgradeUnlocked;
        public event Func<HouseBuildingType, int, float, float, List<RequiredItem>> OnGetRestoreItems;

        public HouseBuildingInfo GetHouseBuildingInfo(HouseBuildingType type)
        {
            houseBuildingInfoDictionary.TryGetValue(type, out var info);
            return info;
        }

        public bool CanUpgradeBuilding(HouseBuilding building) => OnCanUpgradeBuilding != null ? OnCanUpgradeBuilding(building) : false;
        public bool IsUpgradeUnlocked(HouseBuilding building) => OnIsUpgradeUnlocked != null ? OnIsUpgradeUnlocked(building) : false;
        public List<RequiredItem> GetRestoreItems(HouseBuildingType type, int level, float restoreHealth, float maxHealth) => OnGetRestoreItems?.Invoke(type, level, restoreHealth, maxHealth);

        public int GetBuildingsCount(HouseBuildingType type, int level) => ActiveBuildings.Where(b => b.Type == type && b.Level >= level).Count();

        public HouseBuilding GetHouseBuildingByType(HouseBuildingType type) => ActiveBuildings.FirstOrDefault(b => b.Type == type);

        public void AddActiveHouseBuilding(HouseBuilding houseBuilding)
        {
            activeBuildings.Add(houseBuilding);
            if(houseBuilding.Type == HouseBuildingType.TownHall)
            {
                townhall = houseBuilding;
            }
            houseBuilding.OnStateChanged += OnStateChanged;
            houseBuilding.OnStartBuildingProcess += OnStartBuildingProcessHandler;
            houseBuilding.OnCompleteBuildingProcess += OnCompleteBuildingProcessHandler;
            OnActiveBuildingsChanged?.Invoke();
        }

        public void RemoveActiveHouseBuilding(HouseBuilding houseBuilding)
        {
            activeBuildings.Remove(houseBuilding);
            houseBuilding.OnStateChanged -= OnStateChanged;
            houseBuilding.OnStartBuildingProcess -= OnStartBuildingProcessHandler;
            houseBuilding.OnCompleteBuildingProcess -= OnCompleteBuildingProcessHandler;
            OnActiveBuildingsChanged?.Invoke();
        }
        
        public void SetCitizensCount(int allCitizens, int busyCitizens)
        {
            AllCitizensCount = allCitizens;
            BusyCitizensCount = busyCitizens;
            OnCitizensCountChanged?.Invoke();
        }

        private void OnStateChanged() => OnHouseStateChanged?.Invoke();

        private void OnStartBuildingProcessHandler(HouseBuilding houseBuilding) => OnStartBuildingProcess?.Invoke(houseBuilding);
        private void OnCompleteBuildingProcessHandler(HouseBuilding houseBuilding) => OnCompleteBuildingProcess?.Invoke(houseBuilding);
    }

    [Serializable]
    public class HouseBuildingInfo
    {
        public Sprite icon;
        public string descriptionText;
        public HouseLevelInfo[] houseLevelInfos;

        public HouseLevelInfo GetLevelInfo(int level) 
        {
            int index = level - 1;
            if(houseLevelInfos != null && index >= 0 && index < houseLevelInfos.Length)
            {
                return houseLevelInfos[index];
            }
            return null;
        }
        public int MaxLevel => houseLevelInfos.Length;

    }

    [Serializable]
    public class HouseLevelInfo
    {
        public RequiredItem[] requiredItems;
        public RequiredBuilding[] requiredBuildings;
        public int requiredCitizens;
        public int addsCitizens;
    }

    [Serializable]
    public class RequiredBuilding
    {
        public HouseBuildingType type;
        public int level;
        public int count;
    }

    public enum HouseBuildingType
    {
        House_default,
        Forge,
        Shipyard,
        TownHall,
        Alchemy,
        Workshop,
        Tools,
        Fence,
        Tower,
        Gate,
        House_1,
        House_2,
        House_3,
        House_4,
        House_5,
        House_6,
        House_7
    }
}
