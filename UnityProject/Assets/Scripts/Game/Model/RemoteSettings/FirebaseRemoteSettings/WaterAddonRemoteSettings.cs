using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class WaterAddonRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _addonGold;

        public int AddonGold { get => _addonGold; set => _addonGold = value; }
    }
}

