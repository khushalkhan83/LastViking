using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class HealthBarView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Text _valueFront;
        [SerializeField] private Text _valueBack;

        [SerializeField] private Image _valueImage;

        [VisibleObject]
        [SerializeField] private GameObject _watchVideoButtonObject;

        [VisibleObject]
        [SerializeField] private GameObject _buyAddonButtonObject;

        [SerializeField] private Text _coinsCountText;

        [VisibleObject]
        [SerializeField] private GameObject _valueAddonObject;

        [SerializeField] private Image _addonImage;

        [VisibleObject]
        [SerializeField] private GameObject _deltaHealthObject;
        [SerializeField] private Image _deltaHealthImage;
        [SerializeField] private RectTransform _deltaHealthRect;
        [SerializeField] private RectTransform _healthRect;
        [SerializeField] private RectTransform _healthAddonRect;

        [SerializeField] private Color _deltaHealthAddColor;
        [SerializeField] private Color _deltaHealthRemoveColor;

        [SerializeField] private float _deltaSpeedMul;

#pragma warning restore 0649
        #endregion

        public RectTransform RectTransformHealthDelta => _deltaHealthRect;
        public RectTransform RectTransformHealth => _healthRect;
        public RectTransform RectTransformHealthAddon => _healthAddonRect;
        public Color ColorDeltaHealthAdd => _deltaHealthAddColor;
        public Color ColorDeltaHealthRemove => _deltaHealthRemoveColor;

        public float DeltaSpeedMul => _deltaSpeedMul;

        public void SetValueFront(string value) => _valueFront.text = value;
        public void SetValueBack(string value) => _valueBack.text = value;

        public void SetDeltaColor(Color color) => _deltaHealthImage.color = color;

        public void SetIsVisibleWatchVideoButton(bool isVisible) => _watchVideoButtonObject.SetActive(isVisible);
        public void SetIsVisibleBuyAddonButton(bool isVisible) => _buyAddonButtonObject.SetActive(isVisible);
        public void SetIsVisibleValueAddon(bool isVisible) => _valueAddonObject.SetActive(isVisible);
        public void SetIsVisibleDeltaHealth(bool isVisible) => _deltaHealthObject.SetActive(isVisible);
        public void SetLessScale(Vector3 scale) => transform.localScale = scale;

        public void SetValueFillAmount(float value) => _valueImage.fillAmount = value;
        public void SetValueAddonFillAmount(float value) => _addonImage.fillAmount = value;

        public void SetTextBuyCoinsCount(string text) => _coinsCountText.text = text;

        //

        //UI
        public event Action OnAddAddon;
        public void AddAddon() => OnAddAddon?.Invoke();

        //

        public void SetValue(string value)
        {
            SetValueFront(value);
            SetValueBack(value);
        }
    }
}
