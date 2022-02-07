using cakeslice;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using System;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class OutLineCoconutController : IOutLineCoconutController, IController
    {
        [Inject] public OutLineCoconutModel OutLineCoconutModel { get; private set; }
        [Inject] public MaterialsProvider MaterialsProvider { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        private Material OutLineMaterial => MaterialsProvider[MaterialID.Outline];

        void IController.Enable()
        {
            OutLineCoconutModel.OnSelect += OnSelectHandler;
            OutLineCoconutModel.OnDeselect += OnDeselectHandler;
            WorldObjectsModel.OnAdd.AddListener(WorldObjectID.food_coconut, OnAddCoconutHandler);
            WorldObjectsModel.OnRemove.AddListener(WorldObjectID.food_coconut, OnRemoveCoconutHandler);
        }

        void IController.Start() {}

        void IController.Disable()
        {
            OutLineCoconutModel.OnSelect -= OnSelectHandler;
            OutLineCoconutModel.OnDeselect -= OnDeselectHandler;

            if (OutLineCoconutModel.IsSelect)
            {
                Deselect();
            }

            WorldObjectsModel.OnAdd.RemoveListener(WorldObjectID.food_coconut, OnAddCoconutHandler);
            WorldObjectsModel.OnRemove.RemoveListener(WorldObjectID.food_coconut, OnRemoveCoconutHandler);
        }

        private void OnSelectHandler() => Select();
        private void OnDeselectHandler() => Deselect();
        private void OnAddCoconutHandler(WorldObjectModel worldObjectModel) => OnAddCoconut(worldObjectModel);
        private void OnRemoveCoconutHandler(WorldObjectModel worldObjectModel) => OnRemoveCoconut(worldObjectModel);
      

        private void OnAddCoconut(WorldObjectModel worldObjectModel)
        {
            if (OutLineCoconutModel.IsSelect)
            {
                GameObject body = worldObjectModel.gameObject;

                MeshRenderer renderer = body.GetComponentInChildren<MeshRenderer>(true);

                Outline o = body.AddComponent<Outline>();
                o.color = 1;
                o.SetCustomRenderer(renderer);
            }
        }

        private void OnRemoveCoconut(WorldObjectModel worldObjectModel, bool ignoreBlock = false)
        {
            bool block = !OutLineCoconutModel.IsSelect;
            if (!ignoreBlock && block)
            {
                return;
            }

            GameObject body = worldObjectModel.gameObject;

            Outline o = body.GetComponent<Outline>();
            if (o != null)
                Component.Destroy(o);
        }

        private void Select()
        {
            var models = WorldObjectsModel.SaveableObjectModels;
            if (models.ContainsKey(WorldObjectID.food_coconut))
            {
                var coconuts = models[WorldObjectID.food_coconut];
                if (coconuts != null && coconuts.Count > 0)
                {
                    OnAddCoconut(coconuts[0]);
                }
            }
        }

        private void Deselect()
        {
            var models = WorldObjectsModel.SaveableObjectModels;
            if (models.ContainsKey(WorldObjectID.food_coconut))
            {
                var coconuts = models[WorldObjectID.food_coconut];
                if (coconuts != null && coconuts.Count > 0)
                {
                    OnRemoveCoconut(coconuts[0], ignoreBlock: true);
                }
            }
        }
    }
}
