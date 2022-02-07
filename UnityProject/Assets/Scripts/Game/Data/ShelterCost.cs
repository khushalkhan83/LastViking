using CodeStage.AntiCheat.ObscuredTypes;
using System;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class ShelterCost
    {
        [SerializeField] private ObscuredString _name;
        [SerializeField] private ObscuredInt _count;

        public ObscuredString Name => _name;
        public ObscuredInt Count => _count;
    }
}
