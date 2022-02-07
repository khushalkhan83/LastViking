using Extensions;
using UnityEngine;
using Game.Components;


namespace Game.StateMachines.CraftItemTutorial
{
    public class HilightCraftButton : HilightTargetItemBase
    {
        public HilightCraftButton(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        private GameObject CraftButton => CraftViewModel.GetCraftButton();

        #region ICraftItemTutorialState

        public override void Enter()
        {
            base.Enter();
            CraftButton.CheckNull().SafeActivateComponent<TutorialHilightAndAnimation>();
        }
        public override void Exit()
        {
            base.Exit();

            CraftButton.CheckNull()?.SafeDeactivateComponent<TutorialHilightAndAnimation>();
        }

        #endregion
    }
}