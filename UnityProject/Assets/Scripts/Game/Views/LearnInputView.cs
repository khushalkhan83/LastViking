using Core.Views;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class LearnInputView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private List<GameObject> steps;
        [SerializeField] private Button clickButton;
#pragma warning restore 0649
        #endregion

        public List<GameObject> Steps => steps;

        public event Action OnClick;

        private void OnEnable()
        {
            clickButton.onClick.AddListener(() => OnClick?.Invoke());
        }

        private void OnDisable()
        {
            clickButton.onClick.RemoveAllListeners();
        }
    }
}