using Core;
using Extensions;
using Game.Models;
using Game.Components;

namespace Game.StateMachines.UseItemTutorial
{
    public class MainScreen : UseItemTutorialStateBase
    {
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }
        [Inject] public InventoryButtonViewModel InventoryButtonViewModel { get; private set; }

        public MainScreen(UseItemTutorialStateMachine stateMachine) : base(stateMachine) { }
        
        #region IUseItemTutorialState
            
        public override void Enter()
        {
            TutorialSimpleDarkViewModel.SetShow(true); 
            InventoryButtonViewModel.GetButton()?.SafeActivateComponent<TutorialHilight>();
            InventoryButtonViewModel.SetPulseAnimation(true);
        }

        public override void Exit()
        {
            TutorialSimpleDarkViewModel.SetShow(false);
            InventoryButtonViewModel.GetButton()?.SafeDeactivateComponent<TutorialHilight>();
            InventoryButtonViewModel.SetPulseAnimation(false);
        }

        public override void OnInventoryOpened()
        {
            StateMachine.SetState(new HilightItemsInInventory(StateMachine));
        }
        #endregion
    }
}