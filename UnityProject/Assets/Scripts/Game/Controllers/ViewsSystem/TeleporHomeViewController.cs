using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;
using UltimateSurvival;
using UnityEngine;
using DebugActions;

namespace Game.Controllers
{
    public class TeleporHomeViewController : ViewControllerBase<TeleporHomeView>
    {
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public TeleporHomeModel TeleporHomeModel { get; private set; }
        [Inject(true)] public PlayerRespawnPoints PlayerRespawnPoints { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
         
        private PlayerEventHandler PlayerEventHandler => ModelsSystem.Instance._playerEventHandler;

        protected override void Show()
        {
            View.OnApply += OnApply;
            View.OnClose += OnClose;
            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnApply -= OnApply;
            View.OnClose -= OnClose;
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
        }

        private void OnChangeLanguageHandler() => SetLocalization();

        private void OnApply()
        {
            ViewsSystem.Hide(View);
            // var player = PlayerEventHandler.gameObject;

            // Vector3 position = PlayerRespawnPoints.InitPlayerPoint.position;
            // Quaternion rotation = PlayerRespawnPoints.InitPlayerPoint.rotation;

            // player.GetComponent<CharacterController>().Move(position);
            // player.transform.position = position;
            // player.transform.rotation = rotation;
            new TeleportActionGeneric("InitPlace", new Vector3(556.8083f,26.6613f,183.3604f)).DoAction();
        }

        private void OnClose()
        {
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            View.SetTextTitle("Teleport Home");
            View.SetTextDescription("Teleport home immediately?");
            View.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
            View.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
        }

    }
}
