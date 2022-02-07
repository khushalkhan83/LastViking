using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class UIBlockPlayerMoveController : IUIBlockPlayerMoveController, IController
    {
        [Inject] public PlayerMovementModel PlayerMovementModel{get; private set;}
        [Inject] public ViewsStateModel ViewsStateModel{get; private set;}

        void IController.Enable() 
        {
            ViewsStateModel.OnOpenWindowsChanged += OnOpenWindowsPopusChanged;
            ViewsStateModel.OnOpenPopupsChanged += OnOpenWindowsPopusChanged;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            ViewsStateModel.OnOpenWindowsChanged -= OnOpenWindowsPopusChanged;
            ViewsStateModel.OnOpenPopupsChanged -= OnOpenWindowsPopusChanged;
        }

        private void OnOpenWindowsPopusChanged() => PlayerMovementModel.SetBlockPlayerMovement(ViewsStateModel.WindowOrPopupOpened());

    }
}
