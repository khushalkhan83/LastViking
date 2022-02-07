using cakeslice;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class OutLineBlueBarrelController : IOutLineBlueBarrelController, IController
    {
        [Inject] public OutLineBlueBarrelModel OutLineBlueBarrelModel { get; private set; }
        [Inject] public MaterialsProvider MaterialsProvider { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }

        public Material OutLineMaterial => MaterialsProvider[MaterialID.Outline];
        public WorldObjectID SelectableObjectId => WorldObjectID.loot_container_barrel_Day00;

        void IController.Enable()
        {
            OutLineBlueBarrelModel.OnSelect += OnSelectHandler;
            OutLineBlueBarrelModel.OnDeselect += OnDeselectHandler;
            WorldObjectsModel.OnSpawned += OnAddSpawnableHandler;
        }

        void IController.Start()
        {
            if (OutLineBlueBarrelModel.IsSelect)
            {
                Select();
            }
        }

        void IController.Disable()
        {
            OutLineBlueBarrelModel.OnSelect -= OnSelectHandler;
            OutLineBlueBarrelModel.OnDeselect -= OnDeselectHandler;

            if (OutLineBlueBarrelModel.IsSelect)
            {
                Deselect();
            }
        }

        private void OnSelectHandler() => Select();
        private void OnDeselectHandler() => Deselect();

        private void OnAddSpawnableHandler(WorldObjectModel obj)
        {
            if(obj.WorldObjectID == SelectableObjectId) 
                OnSpawnBarrel(obj);
        }

        private void OnSpawnBarrel(WorldObjectModel obj)
        {
            if(waitForSelect == false) return;

            Select();
            waitForSelect = false;
        }

        private bool waitForSelect;

        private void Select()
        {
            var models = new List<WorldObjectModel>();
            bool result = WorldObjectsModel.CreatedWorldObjectsModels.TryGetValue(SelectableObjectId, out models);

            if(result == false)
            {
                "The given key was not present in the dictionary. Wait for Barrel spawn".Log();
                waitForSelect = true;
                return;
            }

            if (models.Count() > 0)
            {
                GameObject body = models.ElementAt(0).gameObject;

                MeshRenderer renderer = models.ElementAt(0).GetComponentInChildren<MeshRenderer>(true);

                Outline o = body.AddComponent<Outline>();
                o.color = 1;
                o.SetCustomRenderer(renderer);
            }
        }

        private void Deselect()
        {
            if(WorldObjectsModel == null) return;

            var models = new List<WorldObjectModel>();
            bool result = WorldObjectsModel.CreatedWorldObjectsModels.TryGetValue(SelectableObjectId, out models);

            if(result == false)
            {
                "The given key was not present in the dictionary. Nothing to deselect".Log();
                return;
            }

            if (models.Count() > 0)
            {
                GameObject body = models.ElementAt(0).gameObject;

                Outline o = body.GetComponent<Outline>();
                if (o != null)
                    Component.Destroy(o);
            }
        }
    }
}
