using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class TombRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _takeItemsGold;

        public int TakeItemsGold { get => _takeItemsGold; set => _takeItemsGold = value; }
    }
}

