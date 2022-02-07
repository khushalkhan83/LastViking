using cakeslice;
using Core.Storage;
using Core.Views;
using Game.Models;
using Game.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public class CoastChestInteractable : InteractableObject, IOutlineTarget
    {
        public event Action<IOutlineTarget> OnUpdateRendererList;

        [SerializeField] List<Renderer> _renderers;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }

        public int GetColor()
        {
            return 1;
        }
    }
}
