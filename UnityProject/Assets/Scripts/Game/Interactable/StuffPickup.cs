using Core.Storage;
using Game.Models;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Interactables
{
    public class StuffPickup : InteractableObject, IData
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _root;

#pragma warning restore 0649
        #endregion

        public Transform Root => _root;

        public event Action OnDataInitialize;

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return null;
            }
        }

        public void UUIDInitialize()
        {
            OnDataInitialize?.Invoke();
        }

        public void PickUp()
        {
            Destroy(Root.gameObject);
        }
    }
}
