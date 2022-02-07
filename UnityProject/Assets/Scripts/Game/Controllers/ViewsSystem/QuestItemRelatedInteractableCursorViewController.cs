using Game.Views;
using Core.Controllers;
using UltimateSurvival;
using Core;
using UnityEngine;
using System.Collections.Generic;
using Game.QuestSystem.Map.Extra.Environment;

namespace Game.Controllers
{
    public class QuestItemRelatedInteractableCursorViewController : ViewControllerBase<ItemsRelatedInteractableCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        private QuestDependantInteractable interactable;
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
            interactable = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<QuestDependantInteractable>();
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
            if(interactable.CanUse())
            {
                interactable.Use();
            }
            else
            {
                foreach(var ItemView in requiredItemViews)
                {
                    ItemView.PlayPulseAnimation();
                }
            }
        }
        private void SetupRequiredItems()
        {
            int requiredCount = 1;
            Sprite icon = interactable.GetItemIcon();

            int count = interactable.CanUse() ? requiredCount: 0;
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
