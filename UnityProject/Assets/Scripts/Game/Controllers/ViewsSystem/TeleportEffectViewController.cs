using Game.Views;
using Core.Controllers;
using Core;

namespace Game.Controllers
{
    public class TeleportEffectViewController : ViewControllerBase<TeleportEffectView>
    {
        [Inject] public ViewsSystem ViewsSystem {get;set;}
             
        protected override void Show() 
        {
            View.PlayFadeAimation(OnAnimationFadeComplete);
        }

        protected override void Hide() 
        {
        }

        private void OnAnimationFadeComplete()
        {
            ViewsSystem.Hide(View);
        }

    }
}
