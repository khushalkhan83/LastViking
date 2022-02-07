using Core.Views;
using UnityEngine;

namespace Game.Views
{
    public class PlayerStatusView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Transform _viewsContainer;

#pragma warning restore 0649
        #endregion

        public Transform ViewsContainer => _viewsContainer;

        public void SetScale(Vector3 scale) => transform.localScale = scale;
        public void SetYDistance(Vector3 dist) => transform.position += dist;
    }
}
