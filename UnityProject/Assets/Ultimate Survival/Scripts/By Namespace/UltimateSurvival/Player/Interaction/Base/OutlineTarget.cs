using System;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public class OutlineTarget : MonoBehaviour, IOutlineTarget
    {
        [SerializeField] private int _color;
        [SerializeField] private List<Renderer> _renderers;
        [SerializeField] private bool _isUseWeaponRange;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public int GetColor() => _color;

        public List<Renderer> GetRenderers() => _renderers;

        public bool IsUseWeaponRange() => _isUseWeaponRange;
    }
}