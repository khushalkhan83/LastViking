using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;

namespace Game.Controllers
{
    public class CitizensInfoViewController : ViewControllerBase<CitizensInfoView>
    {
        [Inject] public VillageBuildingModel VillageBuildingModel { get; private set; }

        protected override void Show() 
        {
            VillageBuildingModel.OnCitizensCountChanged += UpdateCitizensCount;
            UpdateCitizensCount();
        }

        protected override void Hide() 
        {
            VillageBuildingModel.OnCitizensCountChanged -= UpdateCitizensCount;
        }

        private void UpdateCitizensCount()
        {
            View.SetCountText($"{VillageBuildingModel.FreeCitizensCount}/{VillageBuildingModel.AllCitizensCount}");
        }

    }
}
