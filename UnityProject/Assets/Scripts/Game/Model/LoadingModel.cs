using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Game.Models
{
    public class LoadingModel : MonoBehaviour
    {
        [Serializable]
        public class LocalizationData
        {
            [ObscuredID(typeof(LocalizationLanguageID))] public ObscuredInt LanguageIDCurrent;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private LocalizationData _data;

        [Tooltip("Space in MB")]
        [SerializeField] private float _requiredSpaceForSave;

#pragma warning restore 0649
        #endregion


        public LocalizationData Data => _data;
        public float RequiredSpaceForSave => _requiredSpaceForSave;

        public string LocalizationUUID { get; } = "3141e40c345d1044789913c215d798d9";
        public string RootPath => Application.persistentDataPath;

        public LocalizationLanguageID LanguageIDCurrent => (LocalizationLanguageID)(int)_data.LanguageIDCurrent;

        public bool HasConfirmedNoSpace { get; private set; }

        public void ConfirmNoSpace() => HasConfirmedNoSpace = true;
    }
}
