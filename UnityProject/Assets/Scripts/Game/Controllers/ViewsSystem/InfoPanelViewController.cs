using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InfoPanelViewController : ViewControllerBase<InfoPanelView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public InfoPanelViewModel InfoPanelViewModel { get; private set; }

        protected override void Show()
        {
            View.SetItemName(LocalizationModel.GetString(InfoPanelViewModel.TitleTextID));
            View.SetItemDescription(LocalizationModel.GetString(InfoPanelViewModel.DescriptionTextID));

            var desctiption = LocalizationModel.GetString(InfoPanelViewModel.DescriptionTextID);
            View.SetDecorActive(string.IsNullOrEmpty(desctiption)); //[REFACTOR]
        }

        protected override void Hide()
        {

        }
    }
}
