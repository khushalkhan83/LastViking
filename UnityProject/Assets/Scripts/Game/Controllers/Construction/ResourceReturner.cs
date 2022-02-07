using EasyBuildSystem.Runtimes.Internal.Part;
using Game.Models;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using static Game.Models.InventoryOperationsModel;

namespace Game.Controllers
{
    public class ResourceReturner : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private PartBehaviour _partBehaviour;
        [SerializeField] private BuildingHealthModel _buildingHealthModel;

#pragma warning restore 0649
        #endregion

        private PartBehaviour PartBehaviour => _partBehaviour;
        private BuildingHealthModel BuildingHealthModel => _buildingHealthModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private BuildingHotBarModel BuildingHotBarModel => ModelsSystem.Instance._buildingHotBarModel;
        private InventoryOperationsModel InventoryOperationsModel => ModelsSystem.Instance._inventoryOperationsModel;
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;
        private BuildingModeModel BuildingModeModel => ModelsSystem.Instance._buildingModel;
        private TutorialBuildingModel TutorialBuildingModel => ModelsSystem.Instance._tutorialBuildingModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            BuildingHealthModel.OnDeath += OnDeath;
        }

        private void OnDisable()
        {
            BuildingHealthModel.OnDeath -= OnDeath;
        }
        #endregion

        private void OnDeath() => ReturnResources();

        private void ReturnResources()
        {
            if (!ReturnResorucesCondition()) return;

            int id = PartBehaviour.Id;
            var buildingItems = ItemsDB.ItemDatabase.Categories.First(c => c.Name == BuildingHotBarModel.BuildingCategory).Items;
            var buildingItem = buildingItems.First(i => i.GetProperty(BuildingHotBarModel.ItemBuildingIndex).Int.Current == id);

            if (buildingItem.Recipe == null) return;

            SavableItem temp = new SavableItem();
            temp.ItemData = buildingItem;
            var price = BuildingModeModel.GetConstructionPrice(temp,IsTutorialConstuction());
            
            InventoryOperationsModel.ItemConfig config = new InventoryOperationsModel.ItemConfig(null, AddedItemDestinationPriority.Inventory, true);

            InventoryOperationsModel.AddItemsToPlayer(price, null, config);    
        }
        private bool IsTutorialConstuction()
        {
            try
            {
                return TutorialBuildingModel.TutorialModeConstructionIds.Contains(BuildingHealthModel.Uniques.FirstOrDefault().UUIDPrefix);
            }
            catch (System.Exception)
            {
                return false;
            }
        }

        private bool ReturnResorucesCondition()
        {
            return PlayerEventHandler.PlayerAttackBuilding;
        }
    }
}
