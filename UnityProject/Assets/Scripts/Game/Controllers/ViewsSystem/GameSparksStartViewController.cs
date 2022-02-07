using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class GameSparksStartViewController : ViewControllerBase<GameSparksStartView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameSparksModel GameSparksModel { get; private set; }
        [Inject] public PlayerProfileModel PlayerProfileModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }
        [Inject] public AnaliticsModel AnaliticsModel { get; private set; }

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);
            if (GameSparksModel.IsHasUserLogined || GameSparksModel.IsHasUserDeviceLogined || GameSparksModel.IsHasUserRegistered)
                LoadData();
            else
                InitUser();

            View.OnStart += OnStart;
            View.OnEditName += OnEditName;
            View.OnSelectGender += OnSelectGender;

            LocalizationModel.OnChangeLanguage += SetLocalization;
            View.OnPrivacyPolicyButtonDown += OnPrivacyPolicyButtonDownHandler;
            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnStart -= OnStart;
            View.OnEditName -= OnEditName;
            View.OnSelectGender -= OnSelectGender;
            View.OnPrivacyPolicyButtonDown -= OnPrivacyPolicyButtonDownHandler;

            LocalizationModel.OnChangeLanguage += SetLocalization;
        }

        private void OnPrivacyPolicyButtonDownHandler()
        {
            Application.OpenURL(View.UrlPrivacyPolicy);
        }

        private void OnSelectGender(bool gender)
        {
            AudioSystem.PlayOnce(AudioID.Button);
            PlayerProfileModel.SetPlayerGender(gender);
            View.ShowGender(gender);
        }

        private void OnEditName()
        {
        }

        private void OnStart()
        {
            View.OnStart -= OnStart;

            AnaliticsModel.Send(AnaliticEventID.PressButtonStart);
            SaveData();
            LoginDevice();
            TutorialModel.StartTutorial();
            Close();
        }

        public void LoginDevice()
        {
            GameSparksModel.SetIsHasUserDeviceLogined(true);

            /* GAMESPARK STUFF */

            //if (GameSparks.Core.GS.Available)
            //{
            //    Debug.Log("Gs start Authenticating Device...");
            //    new DeviceAuthenticationRequest()
            //        .SetDisplayName(PlayerProfileModel.PlayerName)
            //        .Send((response) =>
            //        {
            //            if (!response.HasErrors)
            //            {
            //                Debug.Log("Device Authenticated...");
            //                GameSparksModel.SetIsHasUserDeviceLogined(true);
            //                InitDeviceUser();
            //                bool? isNew = response.NewPlayer;
            //            }
            //            else
            //            {
            //                Debug.Log("start Error Authenticating Device...");
            //            }
            //        });
            //}
            //else
            //{
            //    GameSparksModel.SetIsHasUserDeviceLogined(true);
            //}
        }

        private void InitUserScores()
        {
            /* GAMESPARK STUFF */

            //            new LogEventRequest()
            //.SetEventKey(GameSparksModel.ScorerKey)
            //.SetEventAttribute(GameSparksModel.ScorerAttribute, 0)
            //.Send(response => { });
        }

        private void InitDeviceUser()
        {
            /* GAMESPARK STUFF */

            //            new ChangeUserDetailsRequest()
            //.SetDisplayName(PlayerProfileModel.PlayerName)
            //.Send(response => { });
        }

        private void InitUser()
        {
            PlayerProfileModel.GenerateNextName();
            View.UserName = PlayerProfileModel.PlayerName;
            OnSelectGender(true);
        }

        private void LoadData()
        {
            View.UserName = GameSparksModel.UserName;
        }
        private void SaveData()
        {
            PlayerProfileModel.SetPlayerName(View.UserName);
            GameSparksModel.SetUserName(View.UserName);
            PlayerProfileModel.GenerateNextAvatarIndex();
        }

        private void Close() => StartCoroutine(WaitForClose());

        private IEnumerator WaitForClose()
        {
            yield return new WaitForSecondsRealtime(View.CloseLoginWindowDelay);
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            View.SetTextMale(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Male));
            View.SetTextFemale(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Female));
            View.SetTextStartButton(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_StartBtn));
            View.SetTextNameTitle(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Title));
        }
    }
}
