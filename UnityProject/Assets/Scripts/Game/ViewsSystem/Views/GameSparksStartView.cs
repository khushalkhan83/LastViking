using CodeStage.AntiCheat.ObscuredTypes;
using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class GameSparksStartView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _yourNameText;
        [SerializeField] private Text _startButtonText;
        [SerializeField] private Text _maleActiveText;
        [SerializeField] private Text _femaleActiveText;
        [SerializeField] private Text _maleInactiveText;
        [SerializeField] private Text _femaleInactiveText;
        [SerializeField] private ObscuredString _urlPrivacyPolicy;

        [SerializeField] private InputField _userName;

        [VisibleObject] [SerializeField] private GameObject _maleActive;
        [VisibleObject] [SerializeField] private GameObject _maleInactive;
        [VisibleObject] [SerializeField] private GameObject _femaleActive;
        [VisibleObject] [SerializeField] private GameObject _femaleInactive;

        [SerializeField] private float _closeLoginWindowDelay = 0.25f;

#pragma warning restore 0649
        #endregion
        public string UrlPrivacyPolicy => _urlPrivacyPolicy;

        public string UserName
        {
            set => _userName.text = value;
            get => _userName.text;
        }

        public void SetTextNameTitle(string text) => _yourNameText.text = text;
        public void SetTextStartButton(string text) => _startButtonText.text = text;
        public void SetTextMale(string text)
        {
            _maleActiveText.text = text;
            _maleInactiveText.text = text;
        }
        public void SetTextFemale(string text)
        {
            _femaleActiveText.text = text;
            _femaleInactiveText.text = text;
        }

        public float CloseLoginWindowDelay => _closeLoginWindowDelay;

        public void ShowGender(bool gender)
        {
            _maleActive.SetActive(gender);
            _femaleActive.SetActive(!gender);
        }

        //

        //UI
        public event Action OnStart;
        public void Confirm() => OnStart?.Invoke();

        public event Action OnEditName;
        public void EditName() => OnEditName?.Invoke();

        public event Action<bool> OnSelectGender;
        public void SelectGender(bool g) => OnSelectGender?.Invoke(g);

        public event Action OnPrivacyPolicyButtonDown;
        public void PrivacyPolicyButtonDown() => OnPrivacyPolicyButtonDown?.Invoke();
    }
}
