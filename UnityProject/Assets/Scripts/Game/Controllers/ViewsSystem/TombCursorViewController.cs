using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TombCursorViewController : ViewControllerBase<TombCursorView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnDownHandler();
            }
        }

        private void OnDownHandler() => ViewsSystem.Show<TombPopupView>(ViewConfigID.TombPopup);
    }
}
