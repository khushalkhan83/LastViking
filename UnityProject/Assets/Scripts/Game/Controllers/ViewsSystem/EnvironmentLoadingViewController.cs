using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class EnvironmentLoadingViewController : ViewControllerBase<EnvironmentLoadingView>
    {
        protected override void Hide()
        {
            
        }

        protected override void Show()
        {
            this.transform.SetAsLastSibling();
        }
    }
}
