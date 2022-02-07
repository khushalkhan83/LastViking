using System;
using UnityEngine;

namespace Game.Models.RemoteSettings.Firebase
{
    [Serializable]
    public class SkipObjectivesRemoteSettings : FirebaseRemoteSettingsBase
    {
        [SerializeField] private int _skipGold;

        public int SkipGold { get => _skipGold; set => _skipGold = value; }
    }
}

