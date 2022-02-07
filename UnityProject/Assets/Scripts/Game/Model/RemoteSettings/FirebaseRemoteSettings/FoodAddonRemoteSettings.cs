using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class FoodAddonRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _addonGold;

        public int AddonGold { get => _addonGold; set => _addonGold = value; }
    }
}

