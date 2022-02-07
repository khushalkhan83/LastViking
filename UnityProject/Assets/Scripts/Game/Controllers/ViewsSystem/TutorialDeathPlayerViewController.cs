using Core;
using Core.Controllers;
using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Views;
using UltimateSurvival;

namespace Game.Controllers
{
    public class TutorialDeathPlayerViewController : ViewControllerBase<TutorialDeathPlayerView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LoseViewModel LoseViewModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public PlayerDeathHandler PlayerDeathHandler { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public PlayerStaminaModel PlayerStaminaModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public PlayerRunModel PlayerRunModel { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public JoystickModel JoystickModel { get; private set; }
        [Inject] public PlayerBleedingDamagerModel PlayerBleedingDamagerModel { get; private set; }
        [Inject(true)] public PlayerRespawnPoints PlayerRespawnPoints { get; private set; }
        [Inject] public PlayerRespawnPointsModel PlayerRespawnPointsModel { get; private set; }

        protected override void Show()
        {
            LocalizationModel.OnChangeLanguage += SetLocalization;
            View.OnResurrect += OnResurrect;

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnResurrect -= OnResurrect;
            LocalizationModel.OnChangeLanguage -= SetLocalization;

            StopAllCoroutines();
        }

        private void OnResurrect()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            PlayerBleedingDamagerModel.SetBleeding(false);

            View.OnHide += OnHideRestart;
            ViewsSystem.Hide(View);
        }
        
        private void OnHideRestart(IView view)
        {
            View.OnHide -= OnHideRestart;
            StopAllCoroutines();
            RestartGame();
        }

        private void RestartGame()
        {
            LoseViewModel.PlayAgain();

            Refill();
            LoseViewModel.TutorialResurrect();
            PlayerDeathModel.PrelimRevival();
            PlayerDeathModel.BeginImunitet();
        }

        private void Refill()
        {
            PlayerHealthModel.RefillHealth();
            PlayerFoodModel.RefillFood();
            PlayerWaterModel.RefillWater();
            PlayerStaminaModel.RefillStamina();
            JoystickModel.SetDefaultAxes();
            TouchpadModel.SetDefaultAxes();

            PlayerRunModel.RunStop();
            PlayerRunModel.RunTogglePassive();
            PlayerEventHandler.Jump.ForceStop();
            PlayerEventHandler.Aim.ForceStop();
        }

        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_Title));
            View.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.LoseMenu_TitleTutorial));
            View.SetTextResurrectButtonText(LocalizationModel.GetString(LocalizationKeyID.ShelterUpgradeMenu_Resurrect));
        }
    }
}
