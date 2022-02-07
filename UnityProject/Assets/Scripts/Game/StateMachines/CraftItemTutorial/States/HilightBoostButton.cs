using Extensions;
using UnityEngine;
using Game.Components;
using Core;
using Game.Models;

namespace Game.StateMachines.CraftItemTutorial
{
    public class HilightBoostButton : CraftItemTutorialStateBase
    {
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel {get; private set;}
        [Inject] public CraftViewModel CraftViewModel { get; private set; }

        public HilightBoostButton(CraftItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        private GameObject BoostButton => CraftViewModel.GetBoostButton();

        #region ICraftItemTutorialState

        public override void Enter()
        {
            TutorialSimpleDarkViewModel.SetShow(true);
            BoostButton.CheckNull().SafeActivateComponent<TutorialHilightAndAnimation>();
        }
        public override void Exit()
        {
            TutorialSimpleDarkViewModel.SetShow(false);

            BoostButton.CheckNull()?.SafeDeactivateComponent<TutorialHilightAndAnimation>();
        }

        #endregion
    }
}