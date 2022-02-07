using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class BasicChestRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _buySlotGold;

        public int BuySlotGold { get => _buySlotGold; set => _buySlotGold = value; }
    }
}

