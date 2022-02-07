using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class GameSparksAccountView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [Header("Login")]
        [Space]
        [Header("Input")]
        [SerializeField] private InputField _loginUserName;
        [SerializeField] private InputField _loginUserPass;
        [Header("Result")]
        [SerializeField] private GameObject _loginErrorMessage;
        [SerializeField] private GameObject _loginSuccessMessage;
        [SerializeField] private Image _loginNameIcon;
        [SerializeField] private Image _loginPassIcon;

        [Header("Pass")]
        [SerializeField] private GameObject _loginPassVisibility;
        [SerializeField] private GameObject _rememberLoginPassActive;
        [SerializeField] private GameObject _rememberLoginPassInactive;

        [SerializeField] private Color _successColor;
        [SerializeField] private Color _errorColor;
        [SerializeField] private Color _passiveColor;

        [Space]
        [Space]
        [Header("Register")]
        [Header("Input")]
        [SerializeField] private InputField _registerUserName;
        //[SerializeField] private InputField _registerUserDisplayName;
        [SerializeField] private InputField _registerUserPass;
        [Header("Result")]
        [SerializeField] private GameObject _registerErrorMessage;
        [SerializeField] private GameObject _registerErrorIncorrectPassMessage;
        [SerializeField] private GameObject _registerSuccessMessage;
        [SerializeField] private Image _registerNameIcon;
        [SerializeField] private Image _registerPassIcon;

        [Header("Pass")]
        [SerializeField] private GameObject _registerPassVisibility;
        [SerializeField] private GameObject _rememberRegisterPassActive;
        [SerializeField] private GameObject _rememberRegisterPassInactive;

        [Header("Tabs")]
        [SerializeField] private GameObject _registerTab;
        [SerializeField] private GameObject _loginTab;

        [Header("Params")]
        [SerializeField] private float _closeLoginWindowDelay = 1f;

        [Space]
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _registartionTabRegText;
        [SerializeField] private Text _loginTabRegText;
        [SerializeField] private Text _registartionTabLoginText;
        [SerializeField] private Text _loginTabLoginText;
        [SerializeField] private Text _rememberRegPassText;
        [SerializeField] private Text _rememberLoginPassText;
        [SerializeField] private Text _registartionButtonText;
        [SerializeField] private Text _loginButtonText;
        [SerializeField] private Text _incorrectLoginPass;
        [SerializeField] private Text _registartionLoginTaken;
        [SerializeField] private Text _registartionPassWrong;
        //[SerializeField] private Text _noInternetText;
        [SerializeField] private Text _successLoginText;
        [SerializeField] private Text _successRegisterText;

#pragma warning restore 0649
        #endregion

        public string LoginUserName
        {
            set => _loginUserName.text = value;
            get => _loginUserName.text;
        }
        public string LoginUserPass
        {
            set => _loginUserPass.text = value;
            get => _loginUserPass.text;
        }

        public string RegisterUserName
        {
            set => _registerUserName.text = value;
            get => _registerUserName.text;
        }

        public Color SuccessColor => _successColor;
        public Color ErrorColor => _errorColor;

        //public string RegisterUserDisplayName => _registerUserDisplayName.text;
        public string RegisterUserPass => _registerUserPass.text;

        public bool IsRememberPass => _rememberLoginPassActive.activeSelf;

        public void SetTextTitle(string text) => _titleText.text = text;
        public void SetTextRegistrationTabReg(string text) => _registartionTabRegText.text = text;
        public void SetTextLoginTabReg(string text) => _loginTabRegText.text = text;
        public void SetTextRegistrationTabLogin(string text) => _registartionTabLoginText.text = text;
        public void SetTextLoginTabLogin(string text) => _loginTabLoginText.text = text;
        public void SetTextRememberRegPass(string text) => _rememberRegPassText.text = text;
        public void SetTextRememberLoginPass(string text) => _rememberLoginPassText.text = text;
        public void SetTextRegistartionButton(string text) => _registartionButtonText.text = text;
        public void SetTextLoginButton(string text) => _loginButtonText.text = text;
        public void SetTextIncorrectLoginPass(string text) => _incorrectLoginPass.text = text;
        public void SetTextRegistrationTaken(string text) => _registartionLoginTaken.text = text;
        public void SetTextRegistrationPassWrong(string text) => _registartionPassWrong.text = text;
        public void SetTextSuccessLogin(string text) => _successLoginText.text = text;
        public void SetTextRegistration(string text) => _successRegisterText.text = text;

        public float CloseLoginWindowDelay => _closeLoginWindowDelay;

        public void ResetLoginResult()
        {
            _loginErrorMessage.SetActive(false);
            _loginSuccessMessage.SetActive(false);
            _loginNameIcon.color = SuccessColor;
            _loginPassIcon.color = SuccessColor;
        }

        public void ResetRegisterResult()
        {
            _registerErrorMessage.SetActive(false);
            _registerErrorIncorrectPassMessage.SetActive(false);
            _registerSuccessMessage.SetActive(false);
            _registerNameIcon.color = SuccessColor;
            _registerPassIcon.color = SuccessColor;
        }

        public void ShowLoginError()
        {
            _loginErrorMessage.SetActive(true);
            _loginNameIcon.color = ErrorColor;
            _loginPassIcon.color = ErrorColor;
            _loginSuccessMessage.SetActive(false);
        }

        public void ShowLoginSuccess()
        {
            _loginErrorMessage.SetActive(false);
            _loginNameIcon.color = SuccessColor;
            _loginPassIcon.color = SuccessColor;
            _loginSuccessMessage.SetActive(true);
        }

        public void ShowRegisterError()
        {
            _registerErrorMessage.SetActive(true);
            _registerErrorIncorrectPassMessage.SetActive(false);
            _registerNameIcon.color = ErrorColor;
            _registerPassIcon.color = ErrorColor;
            _registerSuccessMessage.SetActive(false);
        }
        public void ShowRegisterIncorrectPassError()
        {
            _registerErrorMessage.SetActive(false);
            _registerErrorIncorrectPassMessage.SetActive(true);
            _registerNameIcon.color = ErrorColor;
            _registerPassIcon.color = ErrorColor;
            _registerSuccessMessage.SetActive(false);
        }

        public void ShowRegisterSuccess()
        {
            _registerErrorMessage.SetActive(false);
            _registerErrorIncorrectPassMessage.SetActive(false);
            _registerNameIcon.color = SuccessColor;
            _registerPassIcon.color = SuccessColor;
            _registerSuccessMessage.SetActive(true);
        }

        public void ShowLoginPassVisibility(bool on)
        {
            _loginPassVisibility.SetActive(on);
            _loginUserPass.contentType = on? InputField.ContentType.Standard: InputField.ContentType.Password;
            _loginUserPass.ForceLabelUpdate();
        }

        public void ShowRegisterPassVisibility(bool on)
        {
            _registerPassVisibility.SetActive(on);
            _registerUserPass.contentType = on ? InputField.ContentType.Standard : InputField.ContentType.Password;
            _registerUserPass.ForceLabelUpdate();
        }

        public void ShowRememberPass(bool on)
        {
            _rememberLoginPassActive.SetActive(on);
            _rememberLoginPassInactive.SetActive(!on);
            _rememberRegisterPassActive.SetActive(on);
            _rememberRegisterPassInactive.SetActive(!on);
        }

        public void SwitchTab(bool tab)
        {
            _registerTab.SetActive(tab);
            _loginTab.SetActive(!tab);
        }

        //

        //UI
        public event Action OnLogin;
        public void Login() => OnLogin?.Invoke();

        public event Action OnLoginDevice;
        public void LoginDevice() => OnLoginDevice?.Invoke();

        public event Action OnAuthSuccess;
        public void Register() => OnAuthSuccess?.Invoke();

        public event Action OnClose;
        public void CloseView() => OnClose?.Invoke();

        public event Action OnSelectRegisterTab;
        public void SelectRegisterTab() => OnSelectRegisterTab?.Invoke();

        public event Action OnSelectLoginTab;
        public void SelectLoginTab() => OnSelectLoginTab?.Invoke();

        public event Action<bool> OnChangeLoginPasswordVisibility;
        public void ChangeLoginPasswordVisibility(bool on) => OnChangeLoginPasswordVisibility?.Invoke(on);

        public event Action<bool> OnChangeRegisterPasswordVisibility;
        public void ChangeRegisterPasswordVisibility(bool on) => OnChangeRegisterPasswordVisibility?.Invoke(on);

        public event Action<bool> OnChangeRememberPass;
        public void ChangeRememberPass(bool on) => OnChangeRememberPass?.Invoke(on);
    }
}
