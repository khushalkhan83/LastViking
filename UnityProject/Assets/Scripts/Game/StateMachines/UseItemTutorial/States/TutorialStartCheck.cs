using System;
using Core;
using Game.Models;

namespace Game.StateMachines.UseItemTutorial
{
    public class TutorialStartCheck : UseItemTutorialStateBase
    {
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public InventoryModel InventoryModel { get; private set; }
        public TutorialStartCheck(UseItemTutorialStateMachine stateMachine) : base(stateMachine) { }

        #region IUseItemTutorialState
        public override void Enter()
        {
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCellHandler;
            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCellHandler;
            TryStartTutorial();
        }


        public override void Exit()
        {
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCellHandler;
            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCellHandler;
        }

        private void OnChangeCellHandler(CellModel cell) => TryStartTutorial();
            
        #endregion

        private void TryStartTutorial()
        {
            bool result = InventoryOperationsModel.PlayerHasItem(TargetItemName);

            if(result)
            {
                StateMachine.SetState(new MainScreen(StateMachine));
            }
        }
    }
}