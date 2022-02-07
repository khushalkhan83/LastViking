using UnityEngine;
using Game.Models;
using Game.Components;
using Extensions;

namespace Game.StateMachines.UseItemTutorial
{
    public class HilightUseItemButton : HilightItemsInInventoryBase
    {
        public HilightUseItemButton(UseItemTutorialStateMachine stateMachine) : base(stateMachine) { }
        
        
        #region IUseItemTutorialState
        public override void Enter()
        {
            base.Enter();

            var applyButton = GetApplyButton();
            applyButton.SafeActivateComponent<TutorialHilightAndAnimation>();
        }

        public override void Exit()
        {
            base.Exit();
            var applyButton = GetApplyButton();
            applyButton.SafeDeactivateComponent<TutorialHilightAndAnimation>();
        }
        
        #endregion
        private GameObject GetApplyButton() => InventoryPlayerViewModel.GetApplyButton();
    }
}