using Core.Views;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.Views
{
    public class PickUpCursorView : CursorViewExtended
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private Image _amountImage;
        [SerializeField] private Text _timerText;
        [SerializeField] private GameObject _timerTextObject;
        [SerializeField] private Image _autopickupFiller;

#pragma warning restore 0649
        #endregion

        public void SetFillAmount(float value) => _amountImage.fillAmount = value;
        public void SetTimerText(string text) => _timerText.text = text;

        public void SetTimerVisible(bool isVisible) => _timerTextObject.SetActive(isVisible);

        void OnEnable()
        {
            StartCoroutine(AutoPuckUp());
        }

        void OnDisable()
        {
            StopAllCoroutines();
        }

        IEnumerator AutoPuckUp()
        {
            _autopickupFiller.fillAmount = 0f;
            float t = 0;
            float T = 1f;
            while (t<=T)
            {
                t += Time.deltaTime;
                float lerpVal = Mathf.Clamp01(t / T);

                _autopickupFiller.fillAmount = lerpVal;

                yield return null;
            }

            Interact();
        }
    }
}
