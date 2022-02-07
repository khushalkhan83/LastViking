using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System;
using System.Linq;
using UltimateSurvival;

namespace Game.Controllers
{
    public class OpenDoorHealthCursorViewController : ViewControllerBase<OpenDoorHealthCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        protected RestorableObject RestorableObject { get; private set; }

        private void OpenDoor()
        {
            var door = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInChildren<ManualDoorOpener>();
            door?.InteractDoor(PlayerEventHandler.transform);
        }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;

            RestorableObject = PlayerEventHandler.RaycastData.Value.GameObject.GetComponent<RestorableObject>();
            RestorableObject.Health.OnChangeHealth += OnChangeHealthHandler;
            UpdateView();
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;

            RestorableObject.Health.OnChangeHealth -= OnChangeHealthHandler;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnDownHandler();
            }
        }

        private void OnDownHandler() => OpenDoor();

        private void OnChangeHealthHandler() => UpdateView();

        private void UpdateView() => UpdateView(RestorableObject.Health.Health, RestorableObject.Health.HealthMax);

        private void UpdateView(float health, float healthMax)
        {
            View.SetHealthFillAmount(health / healthMax);
            View.SetHealthText($"{health:F0}/{healthMax:F0}");
        }
    }
}
