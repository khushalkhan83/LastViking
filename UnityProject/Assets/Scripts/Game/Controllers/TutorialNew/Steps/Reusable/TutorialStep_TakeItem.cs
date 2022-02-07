using System;
using Core;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_TakeItem : TutorialStepBase
    {
        [Inject] public DropItemModel DropItemModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }

        private readonly string itemID;
        private readonly int tokenConfigID;
        private const WorldObjectID selectableObjectId = WorldObjectID.bag_pickup_floating;

        private TokenTarget token;

        public TutorialStep_TakeItem(TutorialEvent StepStartedEvent, Action OnCompleatedAsSubStep, string itemID, int tokenConfigID): base(StepStartedEvent, OnCompleatedAsSubStep)
        {
            this.itemID = itemID;
            this.tokenConfigID = tokenConfigID;
        }
        
        public override void OnStart()
        {
            DropItemModel.OnItemDroppedFloating += OnItemDroppedFloating;
            WorldObjectsModel.OnAdd.AddListener(selectableObjectId, OnAddWorldObject);
            WorldObjectsModel.OnRemove.AddListener(selectableObjectId, OnRemoveBagPickupHandler);
        }
        public override void OnEnd()
        {
            DropItemModel.OnItemDroppedFloating -= OnItemDroppedFloating;
            WorldObjectsModel.OnAdd.RemoveListener(selectableObjectId, OnAddWorldObject);
            WorldObjectsModel.OnRemove.RemoveListener(selectableObjectId, OnRemoveBagPickupHandler);

            if(token != null) return;
        }

        // copy paste from TutorialStep_GetPickaxe
        private void OnItemDroppedFloating(GameObject item)
        {
            ItemPickup itemPickup = item.GetComponentInChildren<ItemPickup>();
            if(itemPickup != null && itemPickup.ItemToAdd.Name == itemID)
            {
                AddToken(item.gameObject);
                var destroyItemTimeDelay = item.GetComponent<DestroyItemTimeDelay>();
                if(destroyItemTimeDelay != null) destroyItemTimeDelay.RemoveComponentAndSaveIt();
            }
        }   

        private void OnAddWorldObject(WorldObjectModel model)
        {
            ItemPickup itemPickup = model.GetComponentInChildren<ItemPickup>();
            if(itemPickup != null && itemPickup.IsHasItem && itemPickup.ItemToAdd.Name == itemID)
            {
                AddToken(model.gameObject);
            }
        }

        private void AddToken(GameObject obj)
        {
            if(obj.GetComponent<TokenTarget>() == null)
            {
                var tokenTarget = obj.AddComponent<TokenTarget>();
                tokenTarget.TokenConfigId = tokenConfigID;
                tokenTarget.Repaint = true;

                token = tokenTarget;

                obj.SetActive(false);
                obj.SetActive(true);
            }
        }

        private void OnRemoveBagPickupHandler(WorldObjectModel worldObjectModel)
        {
            var itemPickup = worldObjectModel.GetComponentInChildren<ItemPickup>();
            if(itemPickup == null) return;

            bool isTargetItem = ItemsDB.GetItem(itemPickup.ItemToAdd.Id).Name == itemID;
            
            if(!isTargetItem) return;

            RemoveToken(worldObjectModel.gameObject);
            CheckConditions();
        }

        private void RemoveToken(GameObject obj)
        {
            var tokenTarget = obj.GetComponent<TokenTarget>();
            if(tokenTarget != null)
            {
                GameObject.Destroy(tokenTarget);
            }
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }
    }
}