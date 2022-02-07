using cakeslice;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using Game.QuestSystem.Map.Extra;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class OutLineBagPickupController : IOutLineBagPickupController, IController
    {
        [Inject] public OutLineBagPickupModel OutLineBagPickupModel { get; private set; }
        [Inject] public MaterialsProvider MaterialsProvider { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }

        private int tokenConfigId = 11;

        public Material OutLineMaterial => MaterialsProvider[MaterialID.Outline];
        public WorldObjectID SelectableObjectId => WorldObjectID.Bag_Pickup;

        void IController.Enable()
        {
            OutLineBagPickupModel.OnSelect += OnSelectHandler;
            OutLineBagPickupModel.OnDeselect += OnDeselectHandler;
            WorldObjectsModel.OnAdd.AddListener(SelectableObjectId, OnAddBagPickupHandler);
            WorldObjectsModel.OnRemove.AddListener(SelectableObjectId, OnRemoveBagPickupHandler);
        }

        void IController.Start()
        {
            if (OutLineBagPickupModel.IsSelect)
            {
                Select();
            }
        }

        void IController.Disable()
        {
            OutLineBagPickupModel.OnSelect -= OnSelectHandler;
            OutLineBagPickupModel.OnDeselect -= OnDeselectHandler;

            if (OutLineBagPickupModel.IsSelect)
            {
                Deselect();
            }

            WorldObjectsModel.OnAdd.RemoveListener(SelectableObjectId, OnAddBagPickupHandler);
            WorldObjectsModel.OnRemove.RemoveListener(SelectableObjectId, OnRemoveBagPickupHandler);
        }

        private void OnSelectHandler() => Select();
        private void OnDeselectHandler() => Deselect();

        private void OnAddBagPickupHandler(WorldObjectModel worldObjectModel)
        {
            if (OutLineBagPickupModel.IsSelect)
            {
                GameObject body = worldObjectModel.gameObject;

                MeshRenderer renderer = body.GetComponentInChildren<MeshRenderer>(true);

                Outline o = body.AddComponent<Outline>();
                o.color = 1;
                o.SetCustomRenderer(renderer);
                AddToken(body);
            }
        }

        private void OnRemoveBagPickupHandler(WorldObjectModel worldObjectModel)
        {
            if (OutLineBagPickupModel.IsSelect)
            {
                GameObject body = worldObjectModel.gameObject;

                Outline o = body.GetComponent<Outline>();
                if (o != null)
                    Component.Destroy(o);

                RemoveToken(body);
            }
        }

        private void Select()
        {
            var models = WorldObjectsModel.SaveableObjectModels;
            if (models.ContainsKey(SelectableObjectId))
            {
                var bags = models[SelectableObjectId];
                if (bags != null && bags.Count > 0)
                {
                    GameObject body = bags[0].gameObject;

                    MeshRenderer renderer = body.GetComponentInChildren<MeshRenderer>(true);

                    Outline o = body.AddComponent<Outline>();
                    o.color = 1;
                    o.SetCustomRenderer(renderer);
                    AddToken(body);
                }
            }
        }

        private void Deselect()
        {
            var models = WorldObjectsModel.SaveableObjectModels;
            if (models.ContainsKey(SelectableObjectId))
            {
                var bags = models[SelectableObjectId];
                if (bags != null && bags.Count > 0)
                {
                    GameObject body = bags[0].gameObject;

                    Outline o = body.GetComponent<Outline>();
                    if (o != null)
                        Component.Destroy(o);

                    RemoveToken(body);
                }
            }
        }

        private void AddToken(GameObject obj)
        {
            if(obj.GetComponent<TokenTarget>() == null)
            {
                var tokenTarget = obj.AddComponent<TokenTarget>();
                tokenTarget.TokenConfigId = tokenConfigId;
                tokenTarget.Repaint = true;

                obj.SetActive(false);
                obj.SetActive(true);
            }
        }

        private void RemoveToken(GameObject obj)
        {
            var tokenTarget = obj.GetComponent<TokenTarget>();
            if(tokenTarget != null)
            {
                GameObject.Destroy(tokenTarget);
            }
        }
    }
}
