using Core;
using Core.Controllers;
using Game.Audio;
using Game.Interactables;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class ActivateObjectsCursorViewController : ViewControllerBase<ActivateObjectsCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

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

        private void OnDownHandler()
        {
            var activator = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInChildren<ObjectsActivator>();
            activator?.Activate();
        }
    }
}
