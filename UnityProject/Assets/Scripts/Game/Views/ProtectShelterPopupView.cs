using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ProtectShelterPopupView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Button ok;
        [SerializeField] private Button cancel;

#pragma warning restore 0649
        #endregion

        public event Action OnOk;
        public event Action OnCancel;

        private void OnEnable()
        {
            SetButtonAction(ok,() => OnOk.Invoke());
            SetButtonAction(cancel,() => OnCancel.Invoke());
        }

        private void OnDisable()
        {
            RemoveActions(ok);
            RemoveActions(cancel);
        }

        private void SetButtonAction(Button button, Action action)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(action.Invoke);
        }

        private void RemoveActions(Button button)
        {
            button.onClick.RemoveAllListeners();
        }
    }
}
