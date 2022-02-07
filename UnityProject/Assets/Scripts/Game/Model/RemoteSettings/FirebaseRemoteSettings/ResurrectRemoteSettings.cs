using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class ResurrectRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _resurrectGold;

        public int ResurrectGold { get => _resurrectGold; set => _resurrectGold = value; }
    }
}

