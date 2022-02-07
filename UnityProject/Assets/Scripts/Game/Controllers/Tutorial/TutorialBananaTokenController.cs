using System.Collections.Generic;
using UnityEngine;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class TutorialBananaTokenController : ITutorialBananaTokenController, IController
    {
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }

        private List<WorldObjectModel> bananas;
        private int bananaItemId = 55;
        private int tokenConfigId = 13;
        private string tockenID = "banana_token";
        private Vector3 shiftTokenPosition = new Vector3(0, 0.5f, 0);
        private WorldObjectModel currentTokenTarget = null;
        private float closestSqrDistance;
        private float sqrDistance;
        private bool playerHasBanana = false;

        void IController.Enable() 
        {
            GameUpdateModel.OnUpdate += UpdateToken;
            WorldObjectsModel.OnSpawned += OnAddSpawnableHandler;
            InventoryModel.ItemsContainer.OnAddItems += OnAddItems;
            HotBarModel.ItemsContainer.OnAddItems += OnAddItems;
            GetBananasList();
            CheckPlayerHasBanana();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= UpdateToken;
            WorldObjectsModel.OnSpawned -= OnAddSpawnableHandler;
            InventoryModel.ItemsContainer.OnAddItems -= OnAddItems;
            HotBarModel.ItemsContainer.OnAddItems -= OnAddItems;
            Hide();
        }
        
        private void OnAddSpawnableHandler(WorldObjectModel worldObjectModel)
        {
            if(worldObjectModel.WorldObjectID == WorldObjectID.pickup_banana_tree)
            {
                GetBananasList();
            }
        }

        private void GetBananasList()
        {
            WorldObjectsModel.CreatedWorldObjectsModels.TryGetValue(WorldObjectID.pickup_banana_tree, out bananas);
        }

        private void UpdateToken()
        {
            if(!playerHasBanana && bananas != null && bananas.Count > 0)
            {
                WorldObjectModel closestTarget = null;
                closestSqrDistance = float.MaxValue;
                foreach (var banana in bananas)
                {
                    sqrDistance = (PlayerEventHandler.transform.position - banana.transform.position).sqrMagnitude;
                    if (sqrDistance < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistance;
                        closestTarget = banana;
                    }
                }

                if (currentTokenTarget != closestTarget)
                {
                    currentTokenTarget = closestTarget;
                    Hide();
                    if (currentTokenTarget != null)
                        Show(currentTokenTarget.transform.position);
                }
            }
            else
            {
                Hide();
            }
        }

        private void OnAddItems(int itemdId, int count) => CheckPlayerHasBanana();

        private void CheckPlayerHasBanana()
        {
            playerHasBanana = InventoryModel.ItemsContainer.IsHasItem(bananaItemId) || HotBarModel.ItemsContainer.IsHasItem(bananaItemId);
        }

        protected virtual void Show(Vector3 position) => TokensModel.ShowToken(tockenID, tokenConfigId, position + shiftTokenPosition);

        protected virtual void Hide() => TokensModel.HideToken(tockenID);

    }
}
