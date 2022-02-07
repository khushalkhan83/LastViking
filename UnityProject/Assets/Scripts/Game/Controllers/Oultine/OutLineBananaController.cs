using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Models;
using Game.Providers;
using Core;
using Core.Controllers;
using cakeslice;
using System;

namespace Game.Controllers
{
    public class OutLineBananaController : IController, IOutLineBananaController
    {

        [Inject] public OutLineBanana OutLineBananaModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }

        public WorldObjectID SelectableObjectId => WorldObjectID.pickup_banana_tree;

        public void Disable()
        {
            WorldObjectsModel.OnSpawned -= OnAddSpawnableHandler;
            WorldObjectsModel.OnRemoveSpawned -= OnRemoveSpawnableHandler;
            OutLineBananaModel.OnDeselect -= OnDeselectHandler;
            OutLineBananaModel.OnSelect -= OnSelectHandler;

             if (OutLineBananaModel.IsSelect)
            {
                Deselect();
            }
        }

        public void Enable()
        {
            WorldObjectsModel.OnSpawned += OnAddSpawnableHandler;
            WorldObjectsModel.OnRemoveSpawned += OnRemoveSpawnableHandler;
            OutLineBananaModel.OnSelect += OnSelectHandler;
            OutLineBananaModel.OnDeselect += OnDeselectHandler;
        }

        private void OnRemoveSpawnableHandler(WorldObjectModel banana)
        {
            OnRemoveSpawnable(banana);
        }

        private void OnRemoveSpawnable(WorldObjectModel banana, bool ignoreConditionBlock = false)
        {
            if (banana.WorldObjectID != SelectableObjectId) return;

            bool block = !OutLineBananaModel.IsSelect;

            if (!ignoreConditionBlock && block) return;

            Outline o = banana.GetComponent<Outline>();
            if (o != null)
                Component.Destroy(o);
        }

        private void OnAddSpawnableHandler(WorldObjectModel banana)
        {
            OnAddSpawnable(banana);
        }

        private void OnAddSpawnable(WorldObjectModel banana)
        {
            if (banana.WorldObjectID != SelectableObjectId) return;

            if (!OutLineBananaModel.IsSelect) return;

            LODGroup grp = banana.GetComponent<LODGroup>();
            Renderer r = grp.GetLODs()[0].renderers[0];
            Outline o = banana.gameObject.GetComponent<Outline>();
            if (o == null)
                o = banana.gameObject.AddComponent<Outline>();
            o.color = 1;
            o.SetCustomRenderer(r);
        }

        // Start is called before the first frame update
        public void Start() {}
        private void OnSelectHandler() => Select();
        private void OnDeselectHandler() => Deselect();

        private void Select()
        {
            var list = new List<WorldObjectModel>();
            bool result = WorldObjectsModel.CreatedWorldObjectsModels.TryGetValue(SelectableObjectId, out list);

            if(result == false)
            {
                "The given key was not present in the dictionary. Nothing selected".Log();
                return;
            }

            foreach (var banana in list)
            {
                OnAddSpawnable(banana);
            }
        }


        private void Deselect()
        {
            var list = new List<WorldObjectModel>();
            bool result = WorldObjectsModel.CreatedWorldObjectsModels.TryGetValue(SelectableObjectId, out list);

            if(result == false)
            {
                "The given key was not present in the dictionary. Nothing selected".Error();
                return;
            }

            foreach (WorldObjectModel wm in list)
            {
                OnRemoveSpawnable(wm, ignoreConditionBlock: true);
            }
        }
    }
}