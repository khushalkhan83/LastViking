using System;
using UnityEngine;

namespace Game.Controllers
{
    [Serializable]
    public class WaterConfig
    {
        [SerializeField] private Renderer _renderer;
        [SerializeField] private WaterQualityProvider _qualityProvider;

        public Renderer Renderer => _renderer;
        public WaterQualityProvider QualityProvider => _qualityProvider;
    }
}