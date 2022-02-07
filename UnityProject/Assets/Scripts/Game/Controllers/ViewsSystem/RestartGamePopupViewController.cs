using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class RestartGamePopupViewController : ViewControllerBase<RestartGamePopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            SetLocalization();
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
        }

        private void OnChangeLanguageHandler() => SetLocalization();

        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_Title));
            View.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_Description));
            View.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
            View.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
        }
    }
}