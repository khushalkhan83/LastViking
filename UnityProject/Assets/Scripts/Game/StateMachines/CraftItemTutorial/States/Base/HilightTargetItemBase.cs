using Core;
using Extensions;
using Game.Models;
using System.Linq;
using UnityEngine;
using ContainerID = Game.Models.CraftViewModel.ContainerID;
using Game.Components;

namespace Game.StateMachines.CraftItemTutorial
{
    public abstract class HilightTargetItemBase : CraftItemTutorialStateBase
    {
        [Inject] public CraftModel CraftModel { get; private set; } // override property because of InjectionSystem.Inject bug
        [Inject] public CraftViewModel CraftViewModel { get; private set; }
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }
        public HilightTargetItemBase(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        private const ContainerID TargetContainer = ContainerID.Craft;

        private GameObject targetCell;

        #region ICraftItemTutorialState
            
        public override void Enter()
        {
            TutorialSimpleDarkViewModel.SetShow(true);

            targetCell = GetCellView();
            targetCell.CheckNull().SafeActivateComponent<TutorialHilightAndAnimation>();
        }
        public override void Exit()
        {
            TutorialSimpleDarkViewModel.SetShow(false);

            targetCell.CheckNull()?.SafeDeactivateComponent<TutorialHilightAndAnimation>();
        }

        #endregion

        private GameObject GetCellView()
        {
            int cellIndex = GetTargetCellIndex();

            var answer = CraftViewModel.GetCell(TargetContainer, cellIndex);
            return answer;
        }

        // TODO: add null checks
        protected int GetTargetCellIndex()
        {
            var craftItems = CraftModel.GetItemsByCategory(CraftViewModel.CategorySelected);
            var targetCraftItem = craftItems.ToList().Find(x => x != null && x.Name == TargetItemName);

            var itemIndex = craftItems.ToList().IndexOf(targetCraftItem);
            return itemIndex;
        }
    }
}