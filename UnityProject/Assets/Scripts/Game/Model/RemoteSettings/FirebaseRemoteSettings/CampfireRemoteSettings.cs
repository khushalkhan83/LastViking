using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class CampfireRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _buySlotGold;
        [SerializeField] private int _boostGoldPerSlot;

        public int BuySlotGold { get => _buySlotGold; set => _buySlotGold = value; }
        public int BoostGoldPerSlot { get => _boostGoldPerSlot; set => _boostGoldPerSlot = value; }
    }
}

