using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class InfoPanelDefenceViewController : ViewControllerBase<InfoPanelDefenceView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public InfoPanelDefenceViewModel InfoPanelDefenceViewModel { get; private set; }

        protected override void Show()
        {
            var desctiption = LocalizationModel.GetString(InfoPanelDefenceViewModel.DescriptionTextID);
            var isVisibleDecor = string.IsNullOrEmpty(desctiption);
            var item = InfoPanelDefenceViewModel.ItemData.GetProperty("ObjectHealth");

            View.SetDecorActive(isVisibleDecor);
            View.SetDefenceHealth($"{item.Int.Current}");

            View.SetDefenceItemName(LocalizationModel.GetString(InfoPanelDefenceViewModel.TitleTextID));
            View.SetDefenceItemDescription(desctiption);
        }

        protected override void Hide()
        {

        }
    }
}
