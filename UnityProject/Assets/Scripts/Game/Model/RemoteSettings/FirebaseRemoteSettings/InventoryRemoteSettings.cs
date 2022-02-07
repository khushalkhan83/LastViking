using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class InventoryRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _expandSlotGold;
        [SerializeField] private int _expandForeverSlotGold;

        public int ExpandSlotGold { get => _expandSlotGold; set => _expandSlotGold = value; }
        public int ExpandForeverSlotGold { get => _expandForeverSlotGold; set => _expandForeverSlotGold = value; }
    }
}

