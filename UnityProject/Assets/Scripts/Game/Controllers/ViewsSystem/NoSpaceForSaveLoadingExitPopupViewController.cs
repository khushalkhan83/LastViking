using Game.Models;
using Game.Providers;
using Game.Views;
using System.IO;
using UnityEngine;

namespace Game.Controllers
{
    public class NoSpaceForSaveLoadingExitPopupViewController : MonoBehaviour
    {
        [SerializeField] private LoadingModel LoadingModel;
        [SerializeField] private NoSpaceForSaveExitPopupView View;
        [SerializeField] private LocalizationLanguageProvider _languageProvider;

        private float _waiteTime = 10f;
        private bool _updateTimer;
        private float _timePassed;

        #region MonoBehaviour
        private void OnEnable()
        {
            View.OnExit += OnExit;
            SetLocalization();

            View.SetExitButtonInteractable(false);
            _updateTimer = true;
            _timePassed = 0;
            View.SetTextTimerText(_waiteTime.ToString());

            SetLocalization();
        }

        private void OnDisable()
        {
            View.OnExit -= OnExit;
        }
        
        private void Update()
        {
            if(_updateTimer)
            {
                _timePassed += Time.unscaledDeltaTime;
                if(_timePassed > _waiteTime)
                {
                    View.SetExitButtonInteractable(true);
                    View.SetTextTimerText("");
                    _updateTimer = false;
                }
                else
                {
                    int _timer = Mathf.CeilToInt(_waiteTime - _timePassed);
                    View.SetTextTimerText(_timer.ToString());
                }
            }
        }

        #endregion


        private void OnExit()
        {
            Application.Quit();
        }

        private void SetLocalization()
        {
            if (File.Exists(Path.Combine(LoadingModel.RootPath, LoadingModel.LocalizationUUID)))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(Path.Combine(LoadingModel.RootPath, LoadingModel.LocalizationUUID)), LoadingModel.Data);
            }
            View.SetTextMessageText(GetLocalizationString(LoadingModel.LanguageIDCurrent, LocalizationKeyID.NotEnoughStorageSpaceCriticalPopUp_Text));
            View.SetTextExitButtonText(GetLocalizationString(LoadingModel.LanguageIDCurrent, LocalizationKeyID.Close_Game_Button));
        }

        public string GetLocalizationString(LocalizationLanguageID languageID, LocalizationKeyID keyID) => _languageProvider[languageID][keyID];
    }
}
