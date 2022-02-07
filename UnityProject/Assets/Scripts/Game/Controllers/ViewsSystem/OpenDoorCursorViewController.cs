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
    public class OpenDoorCursorViewController : ViewControllerBase<OpenDoorCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        private void OpenDoor()
        {
            var door = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInChildren<ManualDoorOpener>();
            door?.InteractDoor(PlayerEventHandler.transform);
        }

        protected override void Show()
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        protected override void Hide()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            if(PlayerInput.Instance.AttackTap)
            {
                OnDownHandler();
            }
        }

        private void OnDownHandler() => OpenDoor();
    }
}
