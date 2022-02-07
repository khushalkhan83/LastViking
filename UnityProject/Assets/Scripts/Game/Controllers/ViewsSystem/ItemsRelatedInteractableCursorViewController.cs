using Game.Views;
using Core.Controllers;
using Game.Interactables;
using UltimateSurvival;
using Core;
using Game.Models;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

namespace Game.Controllers
{
    public class ItemsRelatedInteractableCursorViewController : ViewControllerBase<ItemsRelatedInteractableCursorView>
    {
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        private ItemsRelatedInteractableBase itemsRelatedInteractable;
        private List<RequiredItemView> requiredItemViews = new List<RequiredItemView>();
        private Transform itemViewsContainer;

        private Transform ItemViewsContainer
        {
            get
            {
                if(itemViewsContainer == null) 
                    itemViewsContainer = GetItemViewsContainer();
                return itemViewsContainer;
            }
        }
        private Transform GetItemViewsContainer()
        {
            if (ViewsSystem.ActiveViews.TryGetValue(ViewConfigID.ResourceMessages, out var views)) 
            {
                ResourceMessagesView resourceMessagesView = null;
                foreach(var view in views){
                    resourceMessagesView = view as ResourceMessagesView;
                }
                if(resourceMessagesView != null)
                    return resourceMessagesView.ContainerContent;
            }

            return View.ItemsContainer;
        }

        protected override void Show() 
        {
            View.OnInteract += OnDownHandler;
            itemsRelatedInteractable = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<ItemsRelatedInteractableBase>();
            SetupRequiredItems();
        }

        protected override void Hide()
        {
            foreach(var ItemView in requiredItemViews)
            {
                Destroy(ItemView.gameObject);
            }
            requiredItemViews.Clear();

            View.OnInteract -= OnDownHandler;
        }

        private void OnDownHandler()
        {
            if(itemsRelatedInteractable.CanUse() && IsEnoughItems())
            {
                if(!EditorGameSettings.IgnoreItemsPrice && itemsRelatedInteractable.RequiredItems != null)
                {
                    foreach (var item in itemsRelatedInteractable.RequiredItems)
                    {
                        var left = HotBarModel.ItemsContainer.RemoveItems(item.Id, item.Count);
                        if (left > 0)
                        {
                            InventoryModel.ItemsContainer.RemoveItems(item.Id, left);
                        }
                    }
                }
                itemsRelatedInteractable.Use();
            }
            else
            {
                foreach(var ItemView in requiredItemViews)
                {
                    ItemView.PlayPulseAnimation();
                }
            }
        }

        private bool IsEnoughItems()
        {   
            if(EditorGameSettings.IgnoreItemsPrice || itemsRelatedInteractable.RequiredItems == null ||  itemsRelatedInteractable.RequiredItems.Length == 0)
                return true;

            return itemsRelatedInteractable.RequiredItems.All(item => InventoryModel.ItemsContainer.GetItemsCount(item.Id) + HotBarModel.ItemsContainer.GetItemsCount(item.Id) >= item.Count);
        }

        private void SetupRequiredItems()
        {
            if(itemsRelatedInteractable.CanUse() &&  itemsRelatedInteractable.RequiredItems != null)
            {
                for(int i = 0 ; i < itemsRelatedInteractable.RequiredItems.Length ; i++)
                {   
                    var item = itemsRelatedInteractable.RequiredItems[i];
                    int requiredCount = item.Count;
                    Sprite icon = ItemsDB.GetItem(item.Id).Icon;
                    int count = InventoryModel.ItemsContainer.GetItemsCount(item.Id) + HotBarModel.ItemsContainer.GetItemsCount(item.Id);
                    bool enoughResources = count >= requiredCount;
                    Color countTextColor = enoughResources ? View.EnoughColor : View.NotEnoughColor;

                    var requiredItemView = Instantiate(View.RequiredItemView, ItemViewsContainer);
                    requiredItemViews.Add(requiredItemView);

                    requiredItemView.gameObject.SetActive(true);
                    requiredItemView.SetIcon(icon);
                    requiredItemView.SetCountText(count + "/" + requiredCount);
                    requiredItemView.SetTextColor(countTextColor);
                    requiredItemView.SetActiveAttentionIcon(!enoughResources);
                    
                }
            }

        }

    }
}
