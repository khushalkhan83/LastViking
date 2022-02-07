using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core.Controllers;
using Core;
using Game.Models;
using Game.Views;
using Core.Views;
using UltimateSurvival;
using Game.Providers;

namespace Game.Controllers
{
    public class MinigameStateController : IMinigameStateController, IController
    {
        [Inject] public MiniGameStateModel minigameModel { get; private set; }
        [Inject] public ControllersModel ControllersModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public MinigameModificatorProvider MinigameModificatorProvider { get; private set; }
        [Inject] public PlayerMovementModel PlayerMovementModel { get; private set; }

        public IView View { get; private set; }

        public void Enable()
        {
            minigameModel.OnStateChange += OnMiniGameStateChange;
        }
        public void Disable()
        {
            minigameModel.OnStateChange -= OnMiniGameStateChange;
        }
        public void Start()
        {

        }


        void OnMiniGameStateChange(bool isMinigame)
        {
            if (isMinigame)
            {
                var modificator = MinigameModificatorProvider[MinigameID.Fishing];
                ControllersModel.ApplyModificator(modificator);
                PlayerMovementModel.SetBlockPlayerMovement(true);

                OpenView();
            }
            else
            {
                var noModificator = MinigameModificatorProvider.GetNullValue();
                ControllersModel.ApplyModificator(noModificator);
                PlayerMovementModel.SetBlockPlayerMovement(false);

                CloseView();
            }
        }

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<MiniGameExitView>(ViewConfigID.MiniGameExit);
                View.OnHide += OnHideHandler;
            }
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }
    }
}