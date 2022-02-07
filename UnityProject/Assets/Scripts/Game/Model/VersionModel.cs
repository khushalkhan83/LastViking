using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class VersionModel : MonoBehaviour, IVersionModel
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredBool IsLegal;
            public ObscuredString VersionLast;

            public void SetIsLegal(bool isLegal)
            {
                IsLegal = isLegal;
                ChangeData();
            }

            public void SetVersionLast(string versionLast)
            {
                VersionLast = versionLast;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private ObscuredString _pathAndroid;
        [SerializeField] private ObscuredString _pathIPhone;
        [SerializeField] private ObscuredString _pathStoreAndroid;
        [SerializeField] private ObscuredString _pathStoreIOs;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public bool IsLegal
        {
            get => _data.IsLegal;
            protected set => _data.SetIsLegal(value);
        }

        public string VersionLast
        {
            get => _data.VersionLast;
            protected set => _data.SetVersionLast(value);
        }

        public string PathVerions =>
#if UNITY_ANDROID
            _pathAndroid;
#elif UNITY_IOS
            _pathIPhone;
#endif

        public string PathStore =>
#if UNITY_ANDROID
            _pathStoreAndroid;
#elif UNITY_IOS
            _pathStoreIOs;
#endif

        public StorageModel StorageModel => _storageModel;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public void SetIsLegal(bool isLegal)
        {
            IsLegal = isLegal;

            if (!isLegal)
            {
                VersionLast = Application.version;
            }
        }
    }
}
