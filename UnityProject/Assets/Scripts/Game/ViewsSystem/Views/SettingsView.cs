using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class SettingsView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _graphicLeveltext;
        [SerializeField] private Text _frameRateLeveltext;
        [SerializeField] private Image _frameRateLevelIcon;
        [SerializeField] private Image _infinityFrameRateLevelIcon;
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Slider _sensativitySlider;

        [SerializeField] private Text _title;
        [SerializeField] private Text _volumeText;
        [SerializeField] private Text _sensativityText;
        [SerializeField] private Text _graphicsText;
        [SerializeField] private Text _frameRateText;
        [SerializeField] private Text _languageText;
        [SerializeField] private Text _restartButtonText;
        [SerializeField] private Text _backButtonText;
        [SerializeField] private Text _languageButtonText;
        [SerializeField] private Dropdown _languageDropdown;
        [SerializeField] private Color _activeColor;
        [SerializeField] private Color _disableColor;
        [SerializeField] private Text _teleportText;
        [SerializeField] private Text _teleportPriceText;
        [SerializeField] private GameObject _teleportObject;

#pragma warning restore 0649
        #endregion

        public void SetGraphicLevelText(string text) => _graphicLeveltext.text = text;
        public void SetFramesLevelText(string text) => _frameRateLeveltext.text = text;
        public void SetIsVisibleFramesLevelText(bool isVisible) => _frameRateLeveltext.gameObject.SetActive(isVisible);
        public void SetIsVisibleInfinityFramesLevelIcon(bool isVisible) => _infinityFrameRateLevelIcon.gameObject.SetActive(isVisible);
        public void SetFramesLevelIcon(Sprite icon) => _frameRateLevelIcon.sprite = icon;
        public void SetVolumeLevel(float level) => _volumeSlider.value = level;
        public void SetSensativityLevel(float level) => _sensativitySlider.value = level;
        public void SetTeleportText(string text) => _teleportText.text = text;
        public void SetTeleportPriceText(string text) => _teleportPriceText.text = text;
        public void SetTeleportButtonActive(bool active) => _teleportObject.SetActive(active);
        public Dropdown LanguageDropdown => _languageDropdown;
        public Color ActiveColor => _activeColor;
        public Color DisableColor => _disableColor;

        //UI
        public event Action OnSave;
        public void Save() => OnSave?.Invoke();

        //UI
        public event Action OnClose;
        public void Close() => OnClose?.Invoke();

        //UI
        public event Action OnShowView;

        //UI
        public event Action OnReset;
        public void Reset() => OnReset?.Invoke();

        //UI
        public event Action OnSwitchGraphicLeft;
        public void SwitchGraphicLeft() => OnSwitchGraphicLeft?.Invoke();

        //UI
        public event Action OnSwitchGraphicRight;
        public void SwitchGraphicRight() => OnSwitchGraphicRight?.Invoke();

        //UI
        public event Action OnSwitchFramesRight;
        public void SwitchFramesRight() => OnSwitchFramesRight?.Invoke();

        //UI
        public event Action OnSwitchFramesLeft;
        public void SwitchFramesLeft() => OnSwitchFramesLeft?.Invoke();

        //UI
        public event Action<float> OnChangeAudioLevel;
        public void ChangeAudioLevel(float level) => OnChangeAudioLevel?.Invoke(_volumeSlider.normalizedValue);
        
        //UI
        public event Action<float> OnChangeSensativityLevel;
        public void ChangeSensativityLevel(float level) => OnChangeSensativityLevel?.Invoke(_sensativitySlider.normalizedValue);

        //UI
        public event Action<int> OnSwitchLanguage;
        public void SwitchLanguage(int index) => OnSwitchLanguage?.Invoke(index);

        public event Action OnInvisiableButtonClick;
        public void InvisiableButtonClick() => OnInvisiableButtonClick?.Invoke();

        public event Action OnTeleportButtonClick;
        public void TeleportButtonClick() => OnTeleportButtonClick?.Invoke();

        public bool IsDropDownInitialized => LanguageDropdown.options.Count != 0;

        public void SetTextTitle(string text) => _title.text = text;
        public void SetTextVolume(string text) => _volumeText.text = text;
        public void SetTextSensativity(string text) => _sensativityText.text = text;
        public void SetTextGraphics(string text) => _graphicsText.text = text;
        public void SetTextFrameRate(string text) => _frameRateText.text = text;
        public void SetTextLanguage(string text) => _languageText.text = text;
        public void SetTextBackButton(string text) => _backButtonText.text = text;
        public void SetTextRestartButton(string text) => _restartButtonText.text = text;
        public void SetTextLanguageButton(string text) => _languageButtonText.text = text;
    }
}
