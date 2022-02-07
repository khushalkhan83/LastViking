using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ShelterHealthBarViewController : ViewControllerBase<ShelterHealthBarView>
    {
        [Inject] public SheltersModel SheltersModel { get; protected set; }

        protected ShelterModel ShelterModel { get; private set; }

        protected override void Show()
        {
            SheltersModel.OnUpgrade += OnUpgradeShelterHandler;
            ShelterModel = SheltersModel.ShelterModel;

            var healthModel = ShelterModel.GetComponentInChildren<IHealth>();
            healthModel.OnChangeHealth += OnChangeHealthHandler;

            UpdateShelterHealthAmount();
        }

        protected override void Hide()
        {
            var healthModel = SheltersModel.ShelterModel.GetComponentInChildren<IHealth>();
            if (healthModel != null)
            {
                healthModel.OnChangeHealth -= OnChangeHealthHandler;
            }
            SheltersModel.OnUpgrade -= OnUpgradeShelterHandler;
        }

        private void OnChangeHealthHandler() => UpdateShelterHealthAmount();

        private void UpdateShelterHealthAmount()
        {
            var healthModel = SheltersModel.ShelterModel.GetComponentInChildren<IHealth>();
            View.SetHealthAmount(healthModel.Health / healthModel.HealthMax);
        }

        private void OnUpgradeShelterHandler(ShelterModel shelterModel)
        {
            var healthModel = ShelterModel.GetComponentInChildren<IHealth>();
            healthModel.OnChangeHealth -= OnChangeHealthHandler;

            healthModel = SheltersModel.ShelterModel.GetComponentInChildren<IHealth>();
            healthModel.OnChangeHealth += OnChangeHealthHandler;

            UpdateShelterHealthAmount();
        }
    }
}
