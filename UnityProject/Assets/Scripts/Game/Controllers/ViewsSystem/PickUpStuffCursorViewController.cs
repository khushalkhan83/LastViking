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
    public class PickUpStuffCursorViewController : ViewControllerBase<PickUpStuffCursorView>
    {
        private PlayerEventHandler _playerEventHandler;
        public PlayerEventHandler PlayerEventHandler => _playerEventHandler ?? (_playerEventHandler = FindObjectOfType<PlayerEventHandler>());

        private ActionsLogModel _actionsLogModel;
        public ActionsLogModel ActionsLogModel => _actionsLogModel ?? (_actionsLogModel = FindObjectOfType<ActionsLogModel>());

        protected override void Show()
        {
            View.OnDown += OnDownHandler;
        }

        protected override void Hide()
        {
            View.OnDown -= OnDownHandler;
        }

        private void OnDownHandler()
        {
            print("PickUpStuffCursorViewController - OnDownHandler()");
            var stuffPickUp = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInChildren<StuffPickup>();

            stuffPickUp.PickUp();
            ActionsLogModel.SendMessage(new MessageAppendCoinData(1));
        }
    }
}
