using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class GameSparksAccountViewController : ViewControllerBase<GameSparksAccountView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public GameSparksModel GameSparksModel { get; private set; }
        [Inject] public PlayerProfileModel PlayerProfileModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);
            LoadUserData();

            View.OnLogin += OnLogin;
            View.OnLoginDevice += OnLoginDevice;
            View.OnAuthSuccess += OnRegister;
            View.OnClose += InstantClose;

            View.OnSelectLoginTab += OnSelectLoginTab;
            View.OnSelectRegisterTab += OnSelectRegisterTab;
            View.OnChangeLoginPasswordVisibility += OnChangeLoginPasswordVisibility;
            View.OnChangeRegisterPasswordVisibility += OnChangeRegisterPasswordVisibility;
            View.OnChangeRememberPass += OnChangeRememberPass;

            LocalizationModel.OnChangeLanguage += SetLocalization;
            SetLocalization();
        }

        protected override void Hide()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            View.OnLogin -= OnLogin;
            View.OnLoginDevice -= OnLoginDevice;
            View.OnAuthSuccess -= OnRegister;
            View.OnClose -= InstantClose;

            View.OnSelectLoginTab -= OnSelectLoginTab;
            View.OnSelectRegisterTab -= OnSelectRegisterTab;
            View.OnChangeLoginPasswordVisibility -= OnChangeLoginPasswordVisibility;
            View.OnChangeRegisterPasswordVisibility -= OnChangeRegisterPasswordVisibility;

            View.ResetLoginResult();
            View.ResetRegisterResult();

            LocalizationModel.OnChangeLanguage += SetLocalization;
        }

        private void InstantClose() => Close(true);

        private void OnRegister()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            //if (GameSparksModel.HasUserRegistered) return;
            if (!CheckUserPass(View.RegisterUserPass))
            {
                View.ShowRegisterIncorrectPassError();
                return;
            }

            /* GAMESPARK STUFF */

            //new RegistrationRequest()
            //.SetDisplayName(View.RegisterUserName)
            //.SetUserName(View.RegisterUserName)
            //.SetPassword(View.RegisterUserPass)
            //.Send((response) =>
            //{

            //    if (!response.HasErrors)
            //    {
            //        View.LoginUserName = View.RegisterUserName;
            //        View.LoginUserPass = View.RegisterUserPass;
            //        View.ShowRegisterSuccess();
            //        //SaveUserData(View.RegisterUserName, View.RegisterUserPass);
            //        //GameSparksModel.HasUserRegistered = true;
            //        OnLogin();
            //        bool? isNew = response.NewPlayer;
            //        if (isNew.HasValue && isNew.Value)
            //            InitUserScores();
            //    }
            //    else
            //    {
            //        //Debug.Log("Error Registering Player... \n " + response.Errors.JSON.ToString());
            //        View.ShowRegisterError();
            //    }
            //});
        }

        private void OnLogin()
        {
            AudioSystem.PlayOnce(AudioID.Button);

            /* GAMESPARK STUFF */

            //new AuthenticationRequest()
            //    .SetUserName(View.LoginUserName)
            //    .SetPassword(View.LoginUserPass)
            //    .Send(response =>
            //    {
            //        if (response.HasErrors)
            //        {
            //            print("Error login: " + response.Errors);
            //            View.ShowLoginError();
            //        }
            //        else
            //        {
            //            //print("GS Auth: " + "; name: " + View.LoginUserName + ", pass: " + View.LoginUserPass + ", display name; " + response.DisplayName);
            //            SaveUserData(View.LoginUserName, View.IsRememberPass ? View.LoginUserPass : string.Empty);
            //            GameSparksModel.SetIsHasUserLogined(true);
            //            GameSparksModel.SetIsHasUserRegistered(true);
            //            if (!View.IsRememberPass) GameSparksModel.SetIsHasUserLogined(false);
            //            View.ShowLoginSuccess();
            //            //InitUserScores();
            //            Close();
            //        }
            //    });
        }

        public void OnLoginDevice()
        {
            /* GAMESPARK STUFF */

            //new DeviceAuthenticationRequest()
            //    .SetDisplayName(View.LoginUserName)
            //    .Send((response) =>
            //    {
            //        if (!response.HasErrors)
            //        {
            //            //Debug.Log("Device Authenticated...");
            //            //View.SetLoginResult(true);
            //            SaveUserData(View.LoginUserName, string.Empty);
            //            GameSparksModel.SetIsHasUserDeviceLogined(true);
            //            InitDeviceUser();
            //            bool? isNew = response.NewPlayer;
            //            if (isNew.HasValue && isNew.Value)
            //                InitUserScores();
            //            Close();
            //        }
            //        else
            //        {
            //            //Debug.Log("Error Authenticating Device...");
            //        }
            //    });
        }

        private void InitUserScores()
        {
            /* GAMESPARK STUFF */

            //new LogEventRequest()
            //    .SetEventKey(GameSparksModel.ScorerKey)
            //    .SetEventAttribute(GameSparksModel.ScorerAttribute, 0)
            //    .Send(response => { });
        }

        private void InitDeviceUser()
        {
            /* GAMESPARK STUFF */

            //new ChangeUserDetailsRequest()
            //    .SetDisplayName(View.LoginUserName)
            //    .Send(response => { });
        }

        private bool CheckUserPass(string pass)
        {
            return !pass.Contains(" ") && pass.Length >= GameSparksModel.MinPassLength;
        }

        private void LoadUserData()
        {
            View.RegisterUserName = GameSparksModel.UserName;
            View.LoginUserName = GameSparksModel.UserName;
            View.LoginUserPass = GameSparksModel.UserPass;
        }

        private void SaveUserData(string userName, string password)
        {
            GameSparksModel.SetUserName(userName);
            GameSparksModel.SetUserPass(password);
            PlayerProfileModel.SetPlayerName(userName);
        }

        private void OnSelectRegisterTab()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            View.SwitchTab(true);
        }

        private void OnSelectLoginTab()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            View.SwitchTab(false);
        }

        private void OnChangeLoginPasswordVisibility(bool on)
        {
            View.ShowLoginPassVisibility(on);
        }
        private void OnChangeRegisterPasswordVisibility(bool on)
        {
            View.ShowRegisterPassVisibility(on);
        }

        private void OnChangeRememberPass(bool on)
        {
            View.ShowRememberPass(on);
        }

        private void Close(bool immed = false)
        {
            if (immed) ViewsSystem.Hide(View);
            else StartCoroutine(WaitForClose());
        }
        private IEnumerator WaitForClose()
        {
            yield return new WaitForSecondsRealtime(View.CloseLoginWindowDelay);
            ViewsSystem.Hide(View);
        }

        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.AccountPopup_Title));
            View.SetTextRegistrationTabReg(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Registration));
            View.SetTextLoginTabReg(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Login));
            View.SetTextRegistrationTabLogin(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Registration));
            View.SetTextLoginTabLogin(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Login));
            View.SetTextRememberRegPass(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_RememberPassChk));
            View.SetTextRememberLoginPass(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_RememberPassChk));
            View.SetTextRegistartionButton(LocalizationModel.GetString(LocalizationKeyID.AccountPopup_RegistrationBtn));
            View.SetTextLoginButton(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_LoginBtn));
            View.SetTextIncorrectLoginPass(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Warning_01));
            View.SetTextRegistrationTaken(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Warning_02));
            View.SetTextRegistrationPassWrong(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Warning_04));
            View.SetTextSuccessLogin(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Success));
            View.SetTextRegistration(LocalizationModel.GetString(LocalizationKeyID.CharacterPopup_Success));
        }
    }
}


