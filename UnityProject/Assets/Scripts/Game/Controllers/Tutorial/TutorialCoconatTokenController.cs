using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialCoconatTokenController : ITutorialCoconatTokenController, IController
    {
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        
        protected virtual WorldObjectID worldObjectID => WorldObjectID.food_coconut;
        private List<WorldObjectModel> worldObjects;
        private int tokenConfigId = 2;
        private string tockenID = "coconat_token";
        private Vector3 shiftTokenPosition = new Vector3(0, 0.5f, 0);
        private WorldObjectModel currentTokenTarget = null;
        private float closestSqrDistance;
        private float sqrDistance;

        void IController.Enable() 
        {
            GameUpdateModel.OnUpdate += UpdateToken;
            WorldObjectsModel.OnAdd.AddListener(worldObjectID, OnAdd);
            GetWorldObjectsList();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= UpdateToken;
            WorldObjectsModel.OnAdd.RemoveListener(worldObjectID, OnAdd);
            Hide();
        }
        
        private void OnAdd(WorldObjectModel worldObjectModel)
        {
            GetWorldObjectsList();
        }

        private void GetWorldObjectsList()
        {
            WorldObjectsModel.SaveableObjectModels.TryGetValue(worldObjectID, out worldObjects);
        }

        private void UpdateToken()
        {
            if(worldObjects != null && worldObjects.Count > 0)
            {
                WorldObjectModel closestTarget = null;
                closestSqrDistance = float.MaxValue;
                foreach (var banana in worldObjects)
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
                else if(currentTokenTarget != null)
                {
                    TokensModel.UpdateToken(tockenID, currentTokenTarget.transform.position + shiftTokenPosition);
                }
            }
            else
            {
                Hide();
            }
        }

        protected virtual void Show(Vector3 position) => TokensModel.ShowToken(tockenID, tokenConfigId, position + shiftTokenPosition);

        protected virtual void Hide() => TokensModel.HideToken(tockenID);

    }
}
