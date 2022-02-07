using Core.Views;
using Game.Views.Cell;
using UnityEngine;

namespace Game.Views
{
    public class HotBarRepairingContainerView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform[] _container;

#pragma warning restore 0649
        #endregion

        public Transform GetCell(int index) => _container[index];
    }
}
