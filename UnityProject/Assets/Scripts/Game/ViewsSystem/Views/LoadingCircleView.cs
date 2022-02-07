using Core.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views
{
    public class LoadingCircleView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GameObject _closeButton;

#pragma warning restore 0649
        #endregion

        public GameObject CloseButton => _closeButton;

        public void SetVisibleCloseButton(bool visible) => _closeButton.SetActive(visible);

        public event Action OnClose;
        public void Close() => OnClose?.Invoke();
    }
}
