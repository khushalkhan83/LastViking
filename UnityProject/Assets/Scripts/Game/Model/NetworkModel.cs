using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Game.Models
{
    public class NetworkModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredString _serverAdress;

#pragma warning restore 0649
        #endregion

        public string ServerAddress => _serverAdress;
        public bool IsHasConnection { get; private set; }

        public event Action OnInternetConnectionStateChange;
        public event Action OnCheckConnection;

        public void UpdateInternetConnectionStatus() => OnCheckConnection?.Invoke();

        public void SetIsHasConnection(bool isHasConnection)
        {
            if (IsHasConnection != isHasConnection)
            {
                IsHasConnection = isHasConnection;
                OnInternetConnectionStateChange?.Invoke();
            }
        }
    }
}
