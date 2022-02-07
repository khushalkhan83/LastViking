using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class IncrementalActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Activatable[] _activatables;

#pragma warning restore 0649
        #endregion

        public Activatable[] Activatables => _activatables;

        public int ActivateIndex { private set; get; }

        public override void OnActivate()
        {
            if (ActivateIndex < Activatables.Length)
                Activatables[ActivateIndex++].OnActivate();
        }
    }
}
