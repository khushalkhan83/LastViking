using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UltimateSurvival
{
    public interface IOutlineTarget
    {
        int GetColor();
        bool IsUseWeaponRange();
        List<Renderer> GetRenderers();
        event Action<IOutlineTarget> OnUpdateRendererList;
    }
}