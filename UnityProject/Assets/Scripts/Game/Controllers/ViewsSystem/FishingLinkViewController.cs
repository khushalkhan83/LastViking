using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;

namespace Game.Controllers
{
    public class FishingLinkViewController : ViewControllerBase<FishingLinkView>
    {
        [Inject] public MoreGamesModel MoreGamesModel {get;set;}
        [Inject] public FishingLinkViewModel ViewModel {get;set;}

        protected override void Show() 
        {
            MoreGamesModel.OnLastRewardReceivedChanged += OnLastRewardReceivedChanged;
            View.OnClick += OnClick;
            View.ShowCoins(!MoreGamesModel.IsLastFishRewardReceived);
        }

        protected override void Hide() 
        {
            MoreGamesModel.OnLastRewardReceivedChanged -= OnLastRewardReceivedChanged;
            View.OnClick -= OnClick;
        }

        private void OnClick()
        {
            ViewModel.Click();
            MoreGamesModel.ClickLastFishLink();
        }

        private void OnLastRewardReceivedChanged()
        {
            View.ShowCoins(!MoreGamesModel.IsLastFishRewardReceived);
        }

    }
}
