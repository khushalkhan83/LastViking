using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TokensViewController : ViewControllerBase<TokensView>
    {
        [Inject] public GameLateUpdateModel GameLateUpdateModel {get;set;}
        protected override void Show()
        {
            GameLateUpdateModel.OnLaterUpdate += OnLateUpdate;
        }

        protected override void Hide()
        {
            GameLateUpdateModel.OnLaterUpdate -= OnLateUpdate;
        }

        private void OnLateUpdate()
        {
            View.UpdateTokens();
        }
    }
}
