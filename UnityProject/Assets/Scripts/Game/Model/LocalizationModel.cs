using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using Game.Providers;
using System;
using UnityEngine;

namespace Game.Models
{
    public class LocalizationModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            [ObscuredID(typeof(LocalizationLanguageID))] public ObscuredInt LanguageIDCurrent;

            public void SetLanguageIDCurrent(int languageIDCurrent)
            {
                LanguageIDCurrent = languageIDCurrent;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private LocalizationLanguageProvider _languageProvider;

        [ObscuredID(typeof(LocalizationLanguageID))]
        [SerializeField] private ObscuredInt _languageIDDefault;
        [SerializeField] private ObscuredBool _isAutoDetectLanguage;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public LocalizationLanguageProvider LanguageProvider => _languageProvider;
        public LocalizationLanguageID LanguageIDDefault => (LocalizationLanguageID)(int)_languageIDDefault;
        public bool IsAutoDetectLanguage => _isAutoDetectLanguage;

        public LocalizationLanguageID LanguageIDCurrent
        {
            get
            {
                return (LocalizationLanguageID)(int)_data.LanguageIDCurrent;
            }
            protected set
            {
                _data.SetLanguageIDCurrent((int)value);
            }
        }

        public StorageModel StorageModel => _storageModel;

        public event Action OnChangeLanguage;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);

            if (LanguageIDCurrent == LocalizationLanguageID.None)
            {
                SetStartupLanguage();
            }
        }

        private void SetStartupLanguage()
        {
            if (IsAutoDetectLanguage)
            {
                LanguageIDCurrent = GetLanguageDetected();
            }
            else
            {
#if UNITY_EDITOR
                if (LanguageIDDefault == LocalizationLanguageID.None)
                {
                    throw new Exception("Not setup for language");
                }
#endif
                LanguageIDCurrent = LanguageIDDefault;
            }
        }

        private LocalizationLanguageID GetLanguageDetected()
        {
            LocalizationLanguageID systemLanguage;
            if (Enum.TryParse(Application.systemLanguage.ToString(), out systemLanguage))
            {
                return systemLanguage;
            }
            else
            {
                return LanguageIDDefault;
            }
        }

        public string GetString(LocalizationKeyID keyID)
        {
            return GetString(LanguageIDCurrent, keyID);
        }

        public string GetString(LocalizationLanguageID languageID, LocalizationKeyID keyID)
        {
#if UNITY_EDITOR
            if (languageID == LocalizationLanguageID.None)
            {
                throw new Exception($"Language not found. languageID[{languageID}]");
            }
            if (!Enum.IsDefined(typeof(LocalizationKeyID), keyID))
            {
                throw new Exception($"KeyID[{keyID}] not defined");
            }
            if (keyID == LocalizationKeyID.None)
            {
                throw new Exception($"key not correct. keyID[{keyID}]");
            }
#endif

            return LanguageProvider[languageID][keyID];
        }

        public void SetLanguage(LocalizationLanguageID languageID)
        {
            LanguageIDCurrent = languageID;
            OnChangeLanguage?.Invoke();
        }
    }
}
