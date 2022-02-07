using System;
using Core.Views;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class StarterPackIconView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text timerField;

        [SerializeField] private Button _iconButton;

        [SerializeField] private GameObject _timerObject;
        [SerializeField] private GameObject _noInetObject;

#pragma warning restore 0649
        #endregion

        public void SetTimer(string value) => timerField.text = value;
        public void SetActiveTimer(bool on) => _timerObject.SetActive(on);
        public void SetActiveNoInetObject(bool on) => _noInetObject.SetActive(on);
        public void SetEnableButton(bool on) => _iconButton.interactable = on;

        public event Action OnClick;
        public void OnIconClick() => OnClick?.Invoke();
    }
}
