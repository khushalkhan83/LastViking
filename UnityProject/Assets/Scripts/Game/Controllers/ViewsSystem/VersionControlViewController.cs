using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class VersionControlViewController : ViewControllerBase<VersionControlPopupView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public VersionModel VersionModel { get; private set; }

        protected override void Show()
        {
            LocalizationModel.OnChangeLanguage += SetLocalization;
            View.OnUpdatePressed += HandleUpdatePressed;
            SetLocalization();
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;
            View.OnUpdatePressed += HandleUpdatePressed;
        }

        private void HandleUpdatePressed() => Application.OpenURL(VersionModel.PathStore);

        private void SetLocalization()
        {
            View.SetTextMesssage(LocalizationModel.GetString(LocalizationKeyID.VersionControlMenu_Description));
            View.SetTextUpdateButton(LocalizationModel.GetString(LocalizationKeyID.VersionControlMenu_UpdateBtn));
            View.SetTextVersion(LocalizationModel.GetString(LocalizationKeyID.VersionControlMenu_Name));
        }
    }
}
