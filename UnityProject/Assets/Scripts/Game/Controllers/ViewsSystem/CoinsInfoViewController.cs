using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class CoinsInfoViewController : ViewControllerBase<CoinsInfoView>
    {
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }

        protected override void Show()
        {
            View.OnAddCoins += OnAddCoinsHandler;

            CoinsModel.OnChange += OnChangeCoinsHandler;
            VillageBuildingModel.OnCitizensCountChanged += OnCitizensCountChanged;

            UpdateCoins();
            UpdateCitizens();
        }

        protected override void Hide()
        {
            View.OnAddCoins -= OnAddCoinsHandler;

            CoinsModel.OnChange -= OnChangeCoinsHandler;
             VillageBuildingModel.OnCitizensCountChanged -= OnCitizensCountChanged;
        }

        private void OnCitizensCountChanged() => UpdateCitizens();

        private void UpdateCitizens() => View.SetTextCitizensValue($"{VillageBuildingModel.FreeCitizensCount}/{VillageBuildingModel.AllCitizensCount}");

        private void OnChangeCoinsHandler() => UpdateCoins();

        private void UpdateCoins() => View.DisplayedCoinsValue = CoinsModel.Coins;

        private void OnAddCoinsHandler() => ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
    }
}
