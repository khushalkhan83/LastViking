using Core.Views;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class BuildingSwitchButtonView : ViewBase
    {
        public event Action OnClick;

        #region Data
#pragma warning disable 0649
        [SerializeField] private Sprite _buildingEnableSprite;
        [SerializeField] private Sprite _buildingDisableSprite;
        [SerializeField] private Image _buttonImage;
        [SerializeField] private GameObject _closeImage;
#pragma warning restore 0649
        #endregion

        public void SetBuildingEnabled(bool enabled) {
            _buttonImage.sprite = enabled ? _buildingEnableSprite : _buildingDisableSprite;
            _closeImage.SetActive(enabled);
        }

        public void OnButtonClick() => OnClick?.Invoke();

    }
}
