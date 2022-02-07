using cakeslice;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Providers;
using System;
using System.Linq;
using UnityEngine;
using UltimateSurvival;
using System.Collections.Generic;

namespace Game.Controllers
{
    public abstract class TutorialOutlineMineableControllerBase : IController
    {
        [Inject] public MinebleElementsModel MinebleElementsModel { get; private set; }
        [Inject] public MaterialsProvider MaterialsProvider { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }


        protected abstract OutLineMinableObjectID outLineMinableObjectID {get;}
        private Material OutLineMaterial => MaterialsProvider[MaterialID.Outline];

       
        private Dictionary<IOutlineTarget, List<Outline>> allParts;
        private List<MinebleElement> elements;
        protected MinebleElement currentTokenTarget;
        protected float closestSqrDistance;
        protected float sqrDistance;

        public virtual void Enable()
        {
            MinebleElementsModel.OnElementAdded += OnElementAddedHandler;
            MinebleElementsModel.OnElementRemoved += OnElementRemovedHandler;
            GameUpdateModel.OnUpdate += OnUpdate;
            allParts = new Dictionary<IOutlineTarget, List<Outline>>();
            GetAllElements();
        }

        public virtual void Start() 
        {
            
        }
        public virtual void Disable()
        {
            MinebleElementsModel.OnElementAdded -= OnElementAddedHandler;
            MinebleElementsModel.OnElementRemoved -= OnElementRemovedHandler;
            GameUpdateModel.OnUpdate -= OnUpdate;
            DeselectAll();
        }

        protected void OnUpdate()
        {
            UpdateOutline();
        }

        protected void UpdateOutline()
        {
            if(elements != null)
            {
                MinebleElement closestTarget = null;
                closestSqrDistance = float.MaxValue;
                foreach (var element in elements)
                {
                    sqrDistance = (PlayerEventHandler.transform.position - element.transform.position).sqrMagnitude;
                    if (sqrDistance < closestSqrDistance)
                    {
                        closestSqrDistance = sqrDistance;
                        closestTarget = element;
                    }
                }

                if (currentTokenTarget != closestTarget)
                {
                    currentTokenTarget = closestTarget;
                    DeselectAll();
                    if (currentTokenTarget != null)
                        SelectElement(currentTokenTarget);
                }
            }
        }

        private void GetAllElements()
        {
            MinebleElementsModel.OutlineObjects.TryGetValue(outLineMinableObjectID, out elements);
        }

        protected void SelectAll()
        {
            allParts = new Dictionary<IOutlineTarget, List<Outline>>();

            if(MinebleElementsModel.OutlineObjects.TryGetValue(outLineMinableObjectID, out elements))
            {
                foreach (var element in elements)
                {
                    SelectElement(element);
                }
            }
        }

        private void SelectElement(MinebleElement minebleElement)
        {
            IOutlineTarget targ = minebleElement.MinableFractures.FirstOrDefault();
            if (targ != null)
                UpdateRenderers(targ);
        }

        private void OnElementAddedHandler(MinebleElement mineble) => GetAllElements();
        private void OnElementRemovedHandler(MinebleElement mineble) { }

        void UpdateRenderers(IOutlineTarget targ)
        {
            if (allParts.ContainsKey(targ))
            {
                for (int i = 0; i < allParts[targ].Count; i++)
                {
                    Component.Destroy(allParts[targ][i]);
                }
                allParts[targ].Clear();
            }
            else
            {
                allParts.Add(targ, new List<Outline>());
            }

            List<Renderer> rrrs = targ.GetRenderers();
            GameObject body = (targ as MonoBehaviour).gameObject;

            if (rrrs.Count > 2)
            {
                rrrs.RemoveAt(0);
            }

            foreach(Renderer r in rrrs)
            {
                Outline o = body.AddComponent<Outline>();
                o.color = 1;
                o.SetCustomRenderer(r);
                allParts[targ].Add(o);
            }
        }


        protected void DeselectAll()
        {
            foreach(KeyValuePair<IOutlineTarget, List<Outline>> pair in allParts)
            {
                for (int i = 0;i< pair.Value.Count; i++)
                {
                    if (pair.Value[i]!=null)
                    {
                        Component.Destroy(pair.Value[i]);
                    }
                }
               pair.Key.OnUpdateRendererList -= UpdateRenderers;
            }
        }
    }
}
