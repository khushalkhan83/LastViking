using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class HouseAttackedInfoViewController : ViewControllerBase<HouseAttackedInfoView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            LocalizationModel.OnChangeLanguage += SetLocalization;
            SetLocalization();
        }

        protected override void Hide()
        {
            LocalizationModel.OnChangeLanguage -= SetLocalization;
        }

        private void SetLocalization() => View.SetTextMessage(LocalizationModel.GetString(LocalizationKeyID.InGameNotif_ShelterAttack));
    }
}
