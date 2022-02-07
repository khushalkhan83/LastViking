using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class AutoSaveInfoViewController : ViewControllerBase<AutoSaveInfoView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public AutosaveModel AutosaveModel { get; private set; }
        [Inject] public EnoughSpaceModel EnoughSpaceModel{ get; set; }


        protected override void Show()
        {
            UpdateText();
        }

        protected override void Hide()
        {

        }

        public string SuccessText { get; } = "Autosaving...";
        public string FailText { get; } = "Autosave may have errors...";

        private void UpdateText()
        {
            bool hasSpace = EnoughSpaceModel.HasEnoughSpace(out bool criticalyNotEnough);
            View.SetDescriptionText(hasSpace ? SuccessText: FailText);
            View.SetColorText(hasSpace ? View.SuccessColor : View.FailColor);
        }
    }
}
