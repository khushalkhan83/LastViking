using Core;
using Core.Controllers;
using Extensions;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class OpenLootCursorViewController : ViewControllerBase<OpenLootCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
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

        private void OnDownHandler()
        {
            PlayerEventHandler.RaycastData.Value?.GameObject.CheckNull()?.GetComponent<LootObject>().Open();
        }
    }
}
