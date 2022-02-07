using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using static Game.Models.InventoryOperationsModel;

namespace Game.Controllers
{
    /* Rename to BuildingHotBarController (check conflict with *enable-controller) */

    public class BuildingController : IBuildingController, IController
    {
        [Inject] public BuildingModeModel BuildingModeModel { get; set; }
        [Inject] public BuildingHotBarModel BuildingHotBarModel { get; set; }
        [Inject] public ItemsDB ItemsDB { get; set; }
        [Inject] public BuildingProcessModel BuildingProcessModel { get; set; }
        [Inject] public StorageModel StorageModel { get; set; }
        [Inject] public AudioSystem AudioSystem { get; set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; set; }
        [Inject] public TutorialBuildingModel TutorialBuildingModel { get; set; }

        void IController.Enable()
        {
            StorageModel.TryProcessing(BuildingHotBarModel._Data);

            BuildingHotBarModel.PartSelectedChanged += PartSelectedChanged;
            BuildingModeModel.BuildingEnabled += OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled += OnBuildingDisabled;
            BuildingModeModel.OnPlacePartButtonPressed += OnPlacePartButtonPressed;
            BuildingProcessModel.OnPartPlacedStable += OnPartPlacedStable;

            BuildingModeModel.OnGetConstructionPrice += GetConstructionPrice;
            InitBuildingItems();
        }

        void IController.Start() 
        {
            
        }

        void IController.Disable()
        {
            BuildingModeModel.BuildingActive = false;

            BuildingHotBarModel.PartSelectedChanged -= PartSelectedChanged;
            BuildingModeModel.BuildingEnabled -= OnBuildingEnabled;
            BuildingModeModel.BuildingDisabled -= OnBuildingDisabled;
            BuildingModeModel.OnPlacePartButtonPressed -= OnPlacePartButtonPressed;
            BuildingProcessModel.OnPartPlacedStable -= OnPartPlacedStable;

            BuildingModeModel.OnGetConstructionPrice -= GetConstructionPrice;
        }

        private void InitBuildingItems()
        {
            var buildingItems = ItemsDB.ItemDatabase.Categories.First(c => c.Name == BuildingHotBarModel.BuildingCategory).Items;
            var grouppedItems = buildingItems.GroupBy(i => i.GetProperty(BuildingHotBarModel.ItemBuildingCategory).ConstructionCategoryID);

            BuildingHotBarModel.Categories.Clear();
            foreach (IGrouping<ConstructionCategoryID, ItemData> group in grouppedItems)
            {
                BuildingHotBarModel.Categories.Add(new BuildingCategory()
                    { CategoryID = group.Key, Items = GetSavableItemGroup(group).ToList() });
            }
        }

        private IEnumerable<SavableItem> GetSavableItemGroup(IEnumerable<ItemData> datas)
        {
            foreach (var item in datas)
            {
                yield return new SavableItem(item);
            }
        }

        private void PartSelectedChanged() => UpdateCurrentItem();
        private void OnBuildingEnabled() => UpdateCurrentItem();
        private void OnBuildingDisabled() => BuildingProcessModel.CancelBuild();
        private void OnPlacePartButtonPressed() => TryPlacePart();

        private void TryPlacePart()
        {
            var buildingItem = BuildingHotBarModel.SelectedItem;

            if (buildingItem.ItemData.Recipe != null)
            {
                var price = BuildingModeModel.GetConstructionPrice(buildingItem,TutorialBuildingModel.TutorialStage);
                bool enoughItems = InventoryOperationsModel.PlayerHasItems(price,showNotEnoughMessage: true);
                if(!enoughItems) return;
            }

            BuildingProcessModel.Place();
        }

        private void UpdateCurrentItem()
        {
            var buildingItem = BuildingHotBarModel.SelectedItem;

            BuildingProcessModel.CancelBuild();
            BuildingProcessModel.SelectBuildItem(GetBuildingIndex(buildingItem));
            BuildingProcessModel.StartBuild();
        }

        private int GetBuildingIndex(SavableItem item) => item.GetProperty(BuildingHotBarModel.ItemBuildingIndex).Int.Current;

        private void OnPartPlacedStable(GameObject part)
        {
            var buildingItem = BuildingHotBarModel.SelectedItem;

            if(TutorialBuildingModel.TutorialStage)
            {
                RegisterTutorialConstruction(part);
            }
            ChargePlayerForConstruction(buildingItem);

            AudioSystem.PlayOnce(AudioID.Construction);
        }

        private void ChargePlayerForConstruction(SavableItem buildingItem)
        {
            if (buildingItem.ItemData.Recipe == null) return;

            var price = BuildingModeModel.GetConstructionPrice(buildingItem,TutorialBuildingModel.TutorialStage);
            InventoryOperationsModel.RemoveItemsFromPlayer(price);
        }

        private void RegisterTutorialConstruction(GameObject go)
        {
            if(!go.TryGetComponent<BuildingHealthModel>(out var model)) return;
            
            try
            {
                TutorialBuildingModel.RegisterTutorialModeConstruction(model.Uniques.FirstOrDefault().UUIDPrefix);
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
            }
        }

        private IEnumerable<ItemInfo> GetConstructionPrice(SavableItem construction, bool useTutorialModificator)
        {
            List<ItemInfo> price = new List<ItemInfo>();
            if(useTutorialModificator)
            {
                foreach (var requiredItem in construction.ItemData.Recipe.RequiredItems)
                {
                    // ItemInfo item = new ItemInfo(requiredItem.Name, (int)(requiredItem.Amount * TutorialBuildingModel.TutorialPriceModifier));
                    ItemInfo item = new ItemInfo(requiredItem.Name, 1);
                    price.Add(item);
                }
            }
            else
            {
                foreach (var requiredItem in construction.ItemData.Recipe.RequiredItems)
                {
                    ItemInfo item = new ItemInfo(requiredItem.Name, requiredItem.Amount);
                    price.Add(item);
                }
            }
            return price;
        }
    }
}
