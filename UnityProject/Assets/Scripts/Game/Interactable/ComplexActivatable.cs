using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class ComplexActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Activatable[] _activatables;

#pragma warning restore 0649
        #endregion

        public Activatable[] Activatables => _activatables;

        public override void OnActivate()
        {
            foreach (var act in Activatables)
                act.OnActivate();
        }
    }
}
