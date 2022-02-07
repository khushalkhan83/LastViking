using Core;
using Extensions;
using Game.Models;
using Game.Components;

namespace Game.StateMachines.CraftItemTutorial
{
    public class MainScreen : CraftItemTutorialStateBase
    {
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }
        [Inject] public CraftButtonViewModel CraftButtonViewModel { get; private set; }

        public MainScreen(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region ICraftItemTutorialState
        public override void Enter()
        {
            TutorialSimpleDarkViewModel.SetShow(true); 
            CraftButtonViewModel.GetButton()?.SafeActivateComponent<TutorialHilight>();
        }

        public override void Exit()
        {
            TutorialSimpleDarkViewModel.SetShow(false);
            CraftButtonViewModel.GetButton()?.SafeDeactivateComponent<TutorialHilight>();
        }

        public override void OnShowCraft() => StateMachine.SetState(GetNextState());

        #endregion


        private ICraftItemTutorialState GetNextState()
        {
            if(ItemIsCrafting && !TeachBoostConditions)
            {
                return new Success(StateMachine);
            }
            else
            {
                return new HilightTargetItem(StateMachine);
            }
        }
    }
}