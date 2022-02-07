using System;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Intro
{
    public class SkipView : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Image image;
        [SerializeField] private GameObject skipTitle;
        #pragma warning restore 0649
        #endregion
        private bool _pressed = false;

        public event Action _OnPointerDown;
        public event Action _OnPointerUp;

        void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
        {
            _OnPointerDown?.Invoke();
        }

        void IPointerUpHandler.OnPointerUp(PointerEventData eventData)
        {
            _OnPointerUp?.Invoke();
        }

        public void SetFill(float fill)
        {
            image.fillAmount = fill;
        }

        public void ShowTitle()
        {
            skipTitle.SetActive(true);
        }

        public void HideTitle()
        {
            skipTitle.SetActive(false);
        }
    }
}