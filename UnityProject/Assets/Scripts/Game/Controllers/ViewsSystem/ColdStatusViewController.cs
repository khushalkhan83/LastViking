using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ColdStatusViewController : ViewControllerBase<ColdStatusView>
    {
        protected bool IsWideScreen => UnityEngine.Camera.main.aspect > 2.1f;

        protected override void Show()
        {

        }

        protected override void Hide()
        {

        }

    }
}
