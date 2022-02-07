using Game.Controllers;
using Game.Models;
using System;
using UnityEngine;

namespace Game.Providers
{
    [Serializable]
    public class WaveInfoData
    {
        [SerializeField] private Sprite _icon;
        [SerializeField] private LocalizationKeyID _descriptionKey;

        public Sprite Icon => _icon;
        public LocalizationKeyID DescriptionKey => _descriptionKey;
    }
}
