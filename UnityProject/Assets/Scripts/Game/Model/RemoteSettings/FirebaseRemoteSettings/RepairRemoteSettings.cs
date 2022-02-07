using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class RepairRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _itemHighGold01;
        [SerializeField] private int _itemHighGold02;
        [SerializeField] private int _itemHighGold03;

        [SerializeField] private int _itemMidGold01;
        [SerializeField] private int _itemMidGold02;
        [SerializeField] private int _itemMidGold03;

        [SerializeField] private int _itemLowGold01;
        [SerializeField] private int _itemLowGold02;
        [SerializeField] private int _itemLowGold03;

        public int ItemHighGold01 { get => _itemHighGold01; set => _itemHighGold01 = value; }
        public int ItemHighGold02 { get => _itemHighGold02; set => _itemHighGold02 = value; }
        public int ItemHighGold03 { get => _itemHighGold03; set => _itemHighGold03 = value; }

        public int ItemMidGold01 { get => _itemMidGold01; set => _itemMidGold01 = value; }
        public int ItemMidGold02 { get => _itemMidGold02; set => _itemMidGold02 = value; }
        public int ItemMidGold03 { get => _itemMidGold03; set => _itemMidGold03 = value; }

        public int ItemLowGold01 { get => _itemLowGold01; set => _itemLowGold01 = value; }
        public int ItemLowGold02 { get => _itemLowGold02; set => _itemLowGold02 = value; }
        public int ItemLowGold03 { get => _itemLowGold03; set => _itemLowGold03 = value; }
    }
}

