using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class ActivatorsActivatable : Activatable
    {
        #region Data
    #pragma warning disable 0649

        [SerializeField] private ObjectsActivator[] activators;

    #pragma warning restore 0649
        #endregion

        public override void OnActivate()
        {
            foreach(var activator in activators)
            {
                activator.Activate();
            }
        }
    }
}