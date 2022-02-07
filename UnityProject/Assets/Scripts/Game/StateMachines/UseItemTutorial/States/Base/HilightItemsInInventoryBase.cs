
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Core;
using Game.Models;
using Game.Components;
using Extensions;

namespace Game.StateMachines.UseItemTutorial
{
    public abstract class HilightItemsInInventoryBase : UseItemTutorialStateBase
    {
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public InventoryPlayerViewModel InventoryPlayerViewModel { get; private set; }
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }

        protected HilightItemsInInventoryBase(UseItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        private List<GameObject> hilightedObjects = new List<GameObject>();

        #region IUseItemTutorialState
        public override void Enter() => EnableClickableItems();
        public override void Exit() => DisableClickableItems();
            
        #endregion

        private void EnableClickableItems()
        {
            TutorialSimpleDarkViewModel.SetShow(true);

            hilightedObjects.Clear();

            HilightItems(ContainerID.Inventory,InventoryModel.ItemsContainer);
            HilightItems(ContainerID.HotBar,HotBarModel.ItemsContainer);
        }

        private void DisableClickableItems()
        {
            TutorialSimpleDarkViewModel.SetShow(false);

            foreach (var view in hilightedObjects)
            {
                view.SafeDeactivateComponent<TutorialHilightAndAnimation>();
            }
            
            hilightedObjects.Clear();
        }

        private void HilightItems(ContainerID containerID, ItemsContainer container)
        {
            List<CellModel> coconutCells = GetItemsCellModels(container);

            foreach (var cellModel in coconutCells)
            {
                var view = GetCellGameObject(cellModel,containerID);
                hilightedObjects.Add(view);
                view.SafeActivateComponent<TutorialHilightAndAnimation>();
            }
        }

        private List<CellModel> GetItemsCellModels(ItemsContainer container)
        {
            return container.Cells.Where(x => x.IsHasItem).Where(x => x.Item.ItemData.Name == TargetItemName).ToList();
        }

        private GameObject GetCellGameObject(CellModel cellModel, ContainerID containerID)
        {
            var view = InventoryPlayerViewModel.GetCell(containerID,cellModel.Id);
            return view;
        }
    }
}