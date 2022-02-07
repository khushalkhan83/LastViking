using Core.Views;
using UnityEngine;

namespace Game.Views
{
    public class ResourceMessagesView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _containerContent;
        [SerializeField] private Transform _containerFloatings;

#pragma warning restore 0649
        #endregion

        public Transform ContainerContent => _containerContent;
        public Transform ContainerFloatings => _containerFloatings;

    }
}
