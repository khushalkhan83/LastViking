using Core;
using Core.Extensions;
using Game.Models;
using Game.Views;
using Core.Controllers;
using Game.StateMachines.CraftItemTutorial;
using static Game.Models.CraftViewModel;
using System;
using System.Collections;

namespace Game.Controllers
{
    public abstract class CraftItemTutorialController : IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InjectionSystem Context { get; private set; }
        [Inject] public CraftModel CraftModel { get; private set; }
        [Inject] public CraftViewModel CraftViewModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private CraftItemTutorialStateMachine _stateMachine;

        protected abstract string TargetItemName {get;}
        protected abstract bool TeachUseBoost {get;}

        private void InitStateMachine()
        {
            _stateMachine = new CraftItemTutorialStateMachine(TargetItemName,TeachUseBoost).Inject(Context);

            _stateMachine.SetState(new Start(_stateMachine));
        }

        void IController.Start() { }
        void IController.Enable()
        {
            CraftModel.Init();

            InitStateMachine();

            ViewsSystem.OnBeginShow.AddListener(ViewConfigID.Craft,OnShowCraft);
            ViewsSystem.OnBeginHide.AddListener(ViewConfigID.Craft,OnHideCraft);

            PlayerHealthModel.OnDeath += OnPlayerDeathHandler;
            PlayerDeathModel.OnRevival += OnPlayerRevivalHandler;
            PlayerDeathModel.OnPreRevival += OnPlayerRevivalHandler;

            CraftModel.OnCraftItemStarted += OnCraftItemStartedHandler;
            CraftModel.OnStartBoost += OnBoost;

            CraftViewModel.OnChangeSelected += OnChangeSelectedHandler;
        }
        void IController.Disable()
        {
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.Craft,OnShowCraft);
            ViewsSystem.OnBeginHide.RemoveListener(ViewConfigID.Craft,OnHideCraft);

            PlayerHealthModel.OnDeath -= OnPlayerDeathHandler;
            PlayerDeathModel.OnRevival -= OnPlayerRevivalHandler;
            PlayerDeathModel.OnPreRevival -= OnPlayerRevivalHandler;

            CraftModel.OnCraftItemStarted -= OnCraftItemStartedHandler;
            CraftModel.OnStartBoost -= OnBoost;

            CraftViewModel.OnChangeSelected -= OnChangeSelectedHandler;

            _stateMachine.SetState(null);
        }


        #region State Machine 
        private void OnShowCraft() => _stateMachine.CurrentState?.OnShowCraft();
        private void OnHideCraft() => _stateMachine.CurrentState?.OnHideCraft();
        private void OnCraftItemStartedHandler(int itemId) => _stateMachine.CurrentState?.OnCraftItemStarted(itemId);
        private void OnBoost() => _stateMachine.CurrentState?.OnBoost();
        private void OnChangeSelectedHandler(CellInfo cellInfo) => _stateMachine.CurrentState?.OnChangeSelectedHandler(cellInfo);
        private void OnPlayerDeathHandler() => _stateMachine.CurrentState?.OnPlayerDeath();
        private void OnPlayerRevivalHandler() => DoActionAfterFrame(() => _stateMachine.CurrentState?.OnPlayerRevival());  // need to wait one frame to show all views on player revival
        #endregion

        private void DoActionAfterFrame(Action action) => CoroutineModel.InitCoroutine(CDoActionAfterFrame(action));

        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterFrame( Action action)
        {
            yield return null;
            action?.Invoke();
        }
    }
}