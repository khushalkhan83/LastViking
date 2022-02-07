using UnityEngine;
using UltimateSurvival;
using System;
using System.Collections.Generic;

namespace Game.Interactables
{
    public class TombInteractable : MonoBehaviour, IOutlineTarget
    {
        public int GetColor()
        {
            return 1;
        }

        public bool IsUseWeaponRange()
        {
            return false;
        }

        [SerializeField]
        List<Renderer> _renderers;

        public event Action<IOutlineTarget> OnUpdateRendererList;

        public List<Renderer> GetRenderers()
        {
            return _renderers;
        }
    }
}
