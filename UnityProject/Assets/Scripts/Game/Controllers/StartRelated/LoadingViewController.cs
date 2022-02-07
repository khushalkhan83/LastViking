using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class LoadingViewController : ViewControllerBase<StartupView>
    {
        protected override void Hide()
        {
            
        }

        protected override void Show()
        {
            View.transform.SetAsLastSibling();
        }
    }
}
