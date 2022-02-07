using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;

namespace Game.Models
{
    public class GameSparksModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredString _userName;
            public ObscuredString _userPass;
            public ObscuredBool _hasUserLogined;
            public ObscuredBool _hasUserDeviceLogined;
            public ObscuredBool _hasUserRegistered;

            public void SetUserName(string name)
            {
                _userName = name;
                ChangeData();
            }

            public void SetUserPass(string userPass)
            {
                _userPass = userPass;
                ChangeData();
            }

            public void SetHasUserLogined(bool logined)
            {
                _hasUserLogined = logined;
                ChangeData();
            }

            public void SetHasUserDeviceLogined(bool deviceLogined)
            {
                _hasUserDeviceLogined = deviceLogined;
                ChangeData();
            }

            public void SetHasUserRegistered(bool registered)
            {
                _hasUserRegistered = registered;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;

        [SerializeField] private string _leaderboardId;
        [SerializeField] private string _scorerKey;
        [SerializeField] private string _scorerAttribute;

        [SerializeField] private int _minPassLength = 6;

#pragma warning restore 0649
        #endregion

        public int MinPassLength => _minPassLength;
        public string LeaderboardId => _leaderboardId;
        public string ScorerKey => _scorerKey;
        public string ScorerAttribute => _scorerAttribute;

        public StorageModel StorageModel => _storageModel;

        public string UserName
        {
            get => _data._userName;
            protected set => _data.SetUserName(value);
        }

        public string UserPass
        {
            get => _data._userPass;
            protected set => _data.SetUserPass(value);
        }

        public bool IsHasUserLogined
        {
            get => _data._hasUserLogined;
            protected set => _data.SetHasUserLogined(value);
        }

        public bool IsHasUserDeviceLogined
        {
            get => _data._hasUserDeviceLogined;
            protected set => _data.SetHasUserDeviceLogined(value);
        }

        public bool IsHasUserRegistered
        {
            get => _data._hasUserRegistered;
            protected set => _data.SetHasUserRegistered(value);
        }

        public event Action OnUserRegistered;
        public event Action OnUserLogined;

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }

        public void Logout()
        {
            SetUserPass(string.Empty);
            //SetIsHasUserDeviceLogined(false);
            SetIsHasUserLogined(false);
            SetIsHasUserRegistered(false);
        }

        public void SetUserPass(string password) => UserPass = password;

        public void SetUserName(string userName) => UserName = userName;

        public void SetIsHasUserDeviceLogined(bool isHasUserDeviceLogined) => IsHasUserDeviceLogined = isHasUserDeviceLogined;

        public void SetIsHasUserRegistered(bool isHasUserRegistered)
        {
            IsHasUserRegistered = isHasUserRegistered;
            if (isHasUserRegistered)
            {
                OnUserRegistered?.Invoke();
            }
        }

        public void SetIsHasUserLogined(bool isHasUserLogined)
        {
            IsHasUserLogined = isHasUserLogined;
            if (isHasUserLogined)
            {
                OnUserLogined?.Invoke();
            }
        }
    }
}
