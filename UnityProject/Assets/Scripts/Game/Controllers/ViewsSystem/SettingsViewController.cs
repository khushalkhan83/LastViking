using Core;
using Core.Controllers;
using DebugActions;
using Game.Audio;
using Game.Models;
using Game.Purchases;
using Game.Purchases.Purchasers;
using Game.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.Controllers
{
    public class SettingsViewController : ViewControllerBase<SettingsView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public DiscordModel DiscordModel { get; private set; }
        [Inject] public SettingsViewModel SettingsViewModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public QualityModel QualityModel { get; private set; }
        [Inject] public ControllersModel ControllersModel { get; private set; }
        [Inject] public TouchpadModel TouchpadModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public AnaliticsUserIDModel AnaliticsUserIDModel { get; private set; }
        [Inject] public PurchasesModel PurchasesModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }

        protected RestartGamePopupView RestartGamePopupView { get; private set; }
        protected DiscordButtonView DiscordButtonView { get; private set; }
        protected QuestionPopupView QuestionTeleportPopupView { get; set; }

        //DO NOT USE DROPDOWN EVER!!!//
        public void Update()
        {
            var toggle = View.LanguageDropdown.GetComponentsInChildren<Toggle>();
            foreach (var _toggle in toggle)
            {
                var cb = _toggle.colors;
                if (_toggle.isOn)
                {
                    cb.normalColor = View.ActiveColor;
                }
                else
                {
                    cb.normalColor = View.DisableColor;
                }
                _toggle.colors = cb;
            }
        }

        protected override void Show()
        {
            AudioSystem.PlayOnce(AudioID.WindowOpen);
            View.SetVolumeLevel(QualityModel.AudioLevel);
            View.SetSensativityLevel(TouchpadModel.SensativityModificator);


            View.OnSave += OnSaveHandler;

            View.OnClose += OnCloseHandler;
            View.OnReset += OnResetHandler;
            View.OnShowView += Update;

            View.OnInvisiableButtonClick += OnInvisiableButtonClickHandler;

            View.OnSwitchGraphicLeft += OnSwitchGraphicLeftHandler;
            View.OnSwitchGraphicRight += OnSwitchGraphicRightHandler;

            View.OnSwitchFramesLeft += OnSwitchFramesLeftHandler;
            View.OnSwitchFramesRight += OnSwitchFramesRightHandler;

            View.OnChangeAudioLevel += OnChangeAudioLevelHandler;
            View.OnSwitchLanguage += OnSwitchLanguageHandle;

            QualityModel.OnChangeQuality += OnChangeQuaityHandler;
            QualityModel.OnChangeFrameRate += OnChangeTargetFrameRateHandler;

            LocalizationModel.OnChangeLanguage += SetLocalization;
            View.OnChangeSensativityLevel += OnChangeSensativityLevelHandler;
            View.OnTeleportButtonClick += OnTeleportButtonClick;

            SettingsViewModel.OnResetGame += OnResetGameModelHandler;

            if (!View.IsDropDownInitialized)
            {
                var languages = Enum.GetValues(typeof(LocalizationLanguageID));
                var ids = new LocalizationLanguageID[languages.Length - 1];
                Array.Copy(languages, 1, ids, 0, languages.Length - 1);
                var names = new string[languages.Length - 1];
                for (int i = 0; i < ids.Length; i++)
                {
                    names[i] = LocalizationModel.GetString(ids[i], LocalizationKeyID.PauseMenu_Language_Current);
                }
                SetLanguageDropdownListOptions(names, GetIndexLanguage(LocalizationModel.LanguageIDCurrent));
            }

            RefreshTargetFrameRate();
            SetLocalization();
            DiscordButtonView = ViewsSystem.Show<DiscordButtonView>(ViewConfigID.DiscordButton);

            ResetInvisiableButtonCounter();
            SetupTeleportButton();
        }

        protected override void Hide()
        {
            View.OnSave -= OnSaveHandler;

            View.OnShowView -= Update;
            QualityModel.OnChangeQuality -= OnChangeQuaityHandler;
            QualityModel.OnChangeFrameRate -= OnChangeTargetFrameRateHandler;

            LocalizationModel.OnChangeLanguage -= SetLocalization;
            View.OnChangeSensativityLevel -= OnChangeSensativityLevelHandler;

            View.OnClose -= OnCloseHandler;
            View.OnReset -= OnResetHandler;
            View.OnSwitchGraphicLeft -= OnSwitchGraphicLeftHandler;
            View.OnSwitchGraphicRight -= OnSwitchGraphicRightHandler;

            View.OnInvisiableButtonClick -= OnInvisiableButtonClickHandler;

            View.OnSwitchFramesLeft -= OnSwitchFramesLeftHandler;
            View.OnSwitchFramesRight -= OnSwitchFramesRightHandler;

            View.OnChangeAudioLevel -= OnChangeAudioLevelHandler;
            View.OnSwitchLanguage -= OnSwitchLanguageHandle;
            View.OnTeleportButtonClick -= OnTeleportButtonClick;

            SettingsViewModel.OnResetGame -= OnResetGameModelHandler;

            if (DiscordButtonView != null)
            {
                ViewsSystem.Hide(DiscordButtonView);

                DiscordButtonView = null;
            }
            HideTeleportPopup();
        }

        private void SetupTeleportButton()
        {
            // if(PlayerScenesModel.PlayerIsOnMainLocation && TutorialModel.IsComplete)
            // {
            //     var teleportPurchase = PurchasesModel.GetInfo<TeleportGold>(PurchaseID.TeleportGold);
            //     View.SetTeleportPriceText(teleportPurchase.CoinCost.ToString());
            //     View.SetTeleportButtonActive(true);
            // }
            // else
            // {
            //     View.SetTeleportButtonActive(false);
            // }
            
            View.SetTeleportButtonActive(false);
        }

        private void OnSaveHandler() => StorageModel.SaveAll();

        public void SetLanguageDropdownListOptions(string[] languages, int current)
        {
            if (View.LanguageDropdown.options.Count == 0)
            {
                var options = new List<Dropdown.OptionData>();
                foreach (var language in languages)
                {
                    options.Add(new Dropdown.OptionData(language));
                }
                View.LanguageDropdown.AddOptions(options);
                View.LanguageDropdown.value = current;
            }
        }

        private int GetIndexLanguage(LocalizationLanguageID localizationLanguageID) => (int)localizationLanguageID - 1;
        private LocalizationLanguageID GetIDLanguage(int index) => (LocalizationLanguageID)index + 1;

        private void OnChangeQuaityHandler() => RefreshGraphicLevelText();
        private void OnChangeTargetFrameRateHandler() => RefreshTargetFrameRate();

        private void OnResetGameModelHandler() => ResetGame();

        private void OnSwitchGraphicRightHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            QualityModel.IncreaseQuality();
        }

        private void OnSwitchGraphicLeftHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            QualityModel.DecreaseQuality();
        }

        private void OnSwitchFramesLeftHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            QualityModel.DecreaseFrameRate();
        }

        private void OnSwitchFramesRightHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            QualityModel.IncreaseFrameRate();
        }

        private void OnChangeAudioLevelHandler(float level) => QualityModel.SetAudioLevel(level);

        private void OnSwitchLanguageHandle(int index)
        {
            LocalizationModel.SetLanguage((LocalizationLanguageID)index + 1);
            View.SetTextLanguageButton(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Language_Current));
        }

        private void RefreshGraphicLevelText() => View.SetGraphicLevelText(LocalizationModel.GetString(GetGraphicLevelText(QualityModel.QualityID)));
        private void RefreshTargetFrameLevelText() => View.SetFramesLevelText(QualityModel.VSyncText);
        private void RefreshTargetFrameLevelIcon() => View.SetFramesLevelIcon(QualityModel.TargetFrameRateIcon);
        private void RefreshTargetFrameRate()
        {
            bool isLastVSyncValue = QualityModel.TargetFrameRateData.VSyncCount == QualityModel.VSyncRateWithHigestFPSPossible;
            View.SetIsVisibleFramesLevelText(!isLastVSyncValue);
            View.SetIsVisibleInfinityFramesLevelIcon(isLastVSyncValue);
            RefreshTargetFrameLevelText();
            RefreshTargetFrameLevelIcon();
        }
        private LocalizationKeyID GetGraphicLevelText(QualityID qualityID)
        {
            switch (qualityID)
            {
                case QualityID.Low:
                    return LocalizationKeyID.PauseMenu_Graphics_Low;
                case QualityID.Medium:
                    return LocalizationKeyID.PauseMenu_Graphics_Medium;
                case QualityID.High:
                    return LocalizationKeyID.PauseMenu_Graphics_High;
            }

            throw new Exception();
        }


        private void OnResetHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ShowRestartGamePopup();
        }

        private void ShowRestartGamePopup()
        {
            RestartGamePopupView = ViewsSystem.Show<RestartGamePopupView>(ViewConfigID.RestartGamePopup);
            RestartGamePopupView.OnClose += OnCloseResetGamePopupHandler;
            RestartGamePopupView.OnApply += OnApplyResetGamePopupHandler;
        }

        private void HideRestartGamePopup()
        {
            if(RestartGamePopupView != null)
            {
                RestartGamePopupView.OnClose -= OnCloseResetGamePopupHandler;
                RestartGamePopupView.OnApply -= OnApplyResetGamePopupHandler;
                ViewsSystem.Hide(RestartGamePopupView);
                RestartGamePopupView = null;
            }
        }

        private void OnCloseResetGamePopupHandler() => HideRestartGamePopup();

        private void OnApplyResetGamePopupHandler()
        {
            HideRestartGamePopup();

            SettingsViewModel.ResetGame();
        }

        private void ResetGame()
        {
            GameTimeModel.ScaleNormal();
            StorageModel.SaveAll();
            if(!TutorialModel.IsComplete)
            {
                TutorialModel.ResetTutorial();
            }
            StorageModel.ClearTracked();
            StorageModel.ClearChanged();
            StorageModel.ClearLateSavable();
            StorageModel.SaveAll();
            ControllersModel.ApplyState(ControllersStateID.None);
            SceneManager.LoadScene(0, LoadSceneMode.Single);
        }

        private void OnCloseHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private void OnChangeSensativityLevelHandler(float level)
        {
            TouchpadModel.SetSensativityModificator(level);
        }

        private void OnTeleportButtonClick()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ShowTeleportPopup();
        }

        private void ShowTeleportPopup()
        {
            QuestionTeleportPopupView = ViewsSystem.Show<QuestionPopupView>(ViewConfigID.QuestionPopupConfig);
            QuestionTeleportPopupView.OnClose += OnCloseTeleportPopupHandler;
            QuestionTeleportPopupView.OnApply += OnApplyTeleportPopupHandler;
            SetQuestionTeleportLocalization();
        }

        private void SetQuestionTeleportLocalization()
        {
            if(QuestionTeleportPopupView != null)
            {
                QuestionTeleportPopupView.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Teleport));
                QuestionTeleportPopupView.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Teleport_Question));
                QuestionTeleportPopupView.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
                QuestionTeleportPopupView.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
            }
        }

        private void HideTeleportPopup()
        {
            if(QuestionTeleportPopupView != null)
            {
                QuestionTeleportPopupView.OnClose -= OnCloseTeleportPopupHandler;
                QuestionTeleportPopupView.OnApply -= OnApplyTeleportPopupHandler;
                ViewsSystem.Hide(QuestionTeleportPopupView);
                QuestionTeleportPopupView = null;
            }
        }

        private void OnCloseTeleportPopupHandler() => HideTeleportPopup();

        private void OnApplyTeleportPopupHandler()
        {
            PurchasesModel.Purchase(PurchaseID.TeleportGold, OnPurchaseTeleportGold);
            HideTeleportPopup();
        }

        private void OnPurchaseTeleportGold(PurchaseResult purchaseResult)
        {
            if (purchaseResult == PurchaseResult.Successfully)
            {
                new TeleportActionGeneric("InitPlace", new Vector3(556.8083f,26.6613f,183.3604f)).DoAction();
                ViewsSystem.Hide(View);
            }
            else
            {
                ViewsSystem.Show<PurchasesView>(ViewConfigID.Purchases);
            }
        }

        private int invisiableButtonCliksCounter;
        private const int invisiableButtonsRequiaredCliks = 5;
        private void OnInvisiableButtonClickHandler()
        {
            invisiableButtonCliksCounter++;
            if(invisiableButtonCliksCounter >= invisiableButtonsRequiaredCliks)
            {
                AnaliticsUserIDModel.ShowPoupup();
                ResetInvisiableButtonCounter();
            }
        }
        private void ResetInvisiableButtonCounter() => invisiableButtonCliksCounter = 0;


        private void SetLocalization()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            RefreshGraphicLevelText();
            RefreshTargetFrameLevelText();
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Name));
            View.SetTextVolume(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Volume));
            View.SetTextSensativity(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Sensitivity));
            View.SetTextGraphics(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Graphics));
            View.SetTextFrameRate(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_SaveBattery));
            View.SetTextLanguage(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Language));
            View.SetTextRestartButton(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_RestartBtn));
            View.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_BackBtn));
            View.SetTeleportText(LocalizationModel.GetString(LocalizationKeyID.PauseMenu_Teleport));
        }
    }
}
