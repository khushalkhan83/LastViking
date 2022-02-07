using Game.Models;
using Game.Providers;
using Game.Views;
using System.IO;
using UnityEngine;

namespace Game.Controllers
{
    public class NoSpaceForSaveLoadingPopupViewController : MonoBehaviour
    {
        [SerializeField] private LoadingModel LoadingModel;
        [SerializeField] private NoSpaceForSavePopupView View;
        [SerializeField] private LocalizationLanguageProvider _languageProvider;

        private void OnEnable()
        {
            View.OnOk += OnOk;
            SetLocalization();
        }

        private void OnDisable()
        {
            View.OnOk -= OnOk;
        }

        private void OnOk()
        {
            View.gameObject.SetActive(false);
            LoadingModel.ConfirmNoSpace();
        }

        private void SetLocalization()
        {
            if (File.Exists(Path.Combine(LoadingModel.RootPath, LoadingModel.LocalizationUUID)))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(Path.Combine(LoadingModel.RootPath, LoadingModel.LocalizationUUID)), LoadingModel.Data);
            }
            View.SetTextMessageText(GetLocalizationString(LoadingModel.LanguageIDCurrent, LocalizationKeyID.NotEnoughStorageSpacePopUp_Text));
            View.SetTextOkButtonText(GetLocalizationString(LoadingModel.LanguageIDCurrent, LocalizationKeyID.ResetWarning_OkBtn));
        }

        public string GetLocalizationString(LocalizationLanguageID languageID, LocalizationKeyID keyID) => _languageProvider[languageID][keyID];
    }
}
