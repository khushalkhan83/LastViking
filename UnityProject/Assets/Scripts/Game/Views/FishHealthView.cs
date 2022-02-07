using Core.Views;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class FishHealthView : ViewBase
    {
        [SerializeField] Slider _healthSlider;
        [SerializeField] Text _heathText;
        [SerializeField] Text _damageText;
        [SerializeField] RectTransform rectTransform;

        private void OnEnable() 
        {
            _damageText.gameObject.SetActive(false);
        }
        
        public void SetHealthSlider(float value) => _healthSlider.value = value;

        public void SetHeathText(string text) => _heathText.text = text;

        public void ShowDamage(int damage)
        {
            _damageText.text = "-" + damage;
            _damageText.gameObject.SetActive(true);
            StartCoroutine(HideDamageText());
        }

        private IEnumerator HideDamageText()
        {
            yield return new WaitForSeconds(0.5f);
            _damageText.gameObject.SetActive(false);
        }

        public void SetScreenPosition(Vector2 viewPortPosition)
        {
            rectTransform.anchoredPosition = viewPortPosition;
        }
    }
}
