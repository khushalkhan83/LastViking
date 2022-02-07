using Core;
using Core.Extensions;
using Game.Models;
using Game.StateMachines.UseItemTutorial;
using Game.Views;
using static Game.Models.InventoryCellsViewModelBase;
using Core.Controllers;
using System.Collections;
using System;

namespace Game.Controllers
{
    public abstract class UseItemTutorialController : IController
    {
        [Inject] public InventoryPlayerViewModel InventoryPlayerViewModel { get; private set; }
        [Inject] public ApplyItemModel ApplyItemModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InjectionSystem Context { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private UseItemTutorialStateMachine _stateMachine;

        protected abstract string TargetItemName {get;}

        private void InitStateMachine()
        {
            _stateMachine = new UseItemTutorialStateMachine(TargetItemName).Inject(Context);

            _stateMachine.SetState(new Start(_stateMachine));
        }

        void IController.Start() { }
        void IController.Enable()
        {
            InitStateMachine();

            ViewsSystem.OnBeginShow.AddListener(ViewConfigID.InventoryPlayer,OnShowInventory);
            ViewsSystem.OnBeginHide.AddListener(ViewConfigID.InventoryPlayer,OnHideInventory);
            
            PlayerHealthModel.OnDeath += OnPlayerDeathHandler;
            PlayerDeathModel.OnRevival += OnPlayerRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnPlayerRevivalHandler; // tutorial revival

            InventoryPlayerViewModel.OnChangeSelected += OnChangeSelectedCellHandler;
            ApplyItemModel.OnPreApplyItem += OnApplyItemHandler;
        }
        void IController.Disable()
        {
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.InventoryPlayer,OnShowInventory);
            ViewsSystem.OnBeginHide.RemoveListener(ViewConfigID.InventoryPlayer,OnHideInventory);
            
            PlayerHealthModel.OnDeath -= OnPlayerDeathHandler;
            PlayerDeathModel.OnRevival -= OnPlayerRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnPlayerRevivalHandler; // tutorial revival

            InventoryPlayerViewModel.OnChangeSelected -= OnChangeSelectedCellHandler;
            ApplyItemModel.OnPreApplyItem -= OnApplyItemHandler;

            _stateMachine.SetState(null);
        }
        
        #region State Machine 
        private void OnShowInventory() => _stateMachine.CurrentState?.OnInventoryOpened();
        private void OnHideInventory() => _stateMachine.CurrentState?.OnInventoryClosed();
        private void OnChangeSelectedCellHandler(CellInfo cellInfo) => _stateMachine.CurrentState?.OnChangeSelectedCell(cellInfo);
        private void OnApplyItemHandler(ItemsContainer container, CellModel cell) => _stateMachine.CurrentState?.OnApplyItem(container,cell);
        private void OnPlayerDeathHandler() => _stateMachine.CurrentState?.OnPlayerDeath();
        private void OnPlayerRevivalHandler() => DoActionAfterFrame(() => _stateMachine.CurrentState?.OnPlayerRevival());
        #endregion

        
        private void DoActionAfterFrame(Action action) => CoroutineModel.InitCoroutine(CDoActionAfterFrame(action));  // need to wait one frame to show all views on player revival

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterFrame( Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}