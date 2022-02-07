using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Game.Views
{
    public class ReplaceButtonView : ViewBase, IPointerDownHandler, IPointerUpHandler
    {
        [SerializeField] Image fillImage = default;
        [SerializeField] Text priceText = default;
        [SerializeField] float fillTime = 0.5f;

        public event Action OnClick;

        private bool pressed = false;
        private float startFillTime = 0;

        private void Update()
        {
            if(pressed)
            {
                float fill = (Time.time - startFillTime) / fillTime;
                if(fill >= 1f)
                {
                    ResetFillPress();
                    OnClick?.Invoke();
                }
                else
                {
                    fillImage.fillAmount = fill;
                }
            }
        }

        public void SetPriceText(string text) => priceText.text = text;

        public void OnPointerDown(PointerEventData eventData)
        {
            if(!pressed)
            {
                pressed = true;
                startFillTime = Time.time;
            }
        }
        public void OnPointerUp(PointerEventData eventData)
        {
            ResetFillPress();
        }

        private void OnEnable() 
        {
            ResetFillPress();
        }

        private void OnDisable() 
        {
            ResetFillPress();
        }

        private void ResetFillPress()
        {
            pressed = false;
            fillImage.fillAmount = 0;
        }

    }
}
