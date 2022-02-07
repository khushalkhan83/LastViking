using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class MilitaryChestRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _buySlotGold;

        public int BuySlotGold { get => _buySlotGold; set => _buySlotGold = value; }
    }
}

