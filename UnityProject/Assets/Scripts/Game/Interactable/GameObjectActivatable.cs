using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Interactables
{
    public class GameObjectActivatable : Activatable
    {
        #region Data
#pragma warning disable 0649

        [VisibleObject] [SerializeField] private GameObject[] _activatableObjects;
        [SerializeField] private bool _setActive;

#pragma warning restore 0649
        #endregion

        public GameObject[] ActivatableObjects => _activatableObjects;
        public bool SetActive => _setActive;

        public override void OnActivate()
        {
            foreach (var obj in ActivatableObjects)
                obj.SetActive(SetActive);
        }
    }
}
