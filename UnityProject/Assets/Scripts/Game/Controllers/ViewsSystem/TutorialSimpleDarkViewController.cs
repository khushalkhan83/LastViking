using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;

namespace Game.Controllers
{
    public class TutorialSimpleDarkViewController : ViewControllerBase<TutorialSimpleDarkView>
    {
        [Inject] public TutorialSimpleDarkViewModel ViewModel {get;private set;}

        protected override void Show() 
        {
            ViewModel.OnPlayAnimation += View.PlayFadeAnimation;
        }

        protected override void Hide() 
        {
            ViewModel.OnPlayAnimation -= View.PlayFadeAnimation;
        }
    }
}