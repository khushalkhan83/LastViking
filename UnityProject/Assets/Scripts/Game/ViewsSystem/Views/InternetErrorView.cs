using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Core.Views;
namespace Game.Views
{
    public class InternetErrorView : ViewBase
    {
        [SerializeField]
        GameObject errorIcon;

        private void OnEnable()
        {
            HideError();
            OnShowError += ShowErrorMessage;
        }

        private void OnDisable()
        {
            OnShowError -= ShowErrorMessage;
        }

        public static void InternetErrorMessage() => OnShowError?.Invoke();       
        static System.Action OnShowError;


        void ShowErrorMessage()
        {
            if (rout == null)
                rout = StartCoroutine(MessageRoutine());
        }

        Coroutine rout = null;

        IEnumerator MessageRoutine()
        {
            ShowError();
            yield return new WaitForSecondsRealtime(3f);
            HideError();
            rout = null;
        }
        void ShowError()
        {
            errorIcon.SetActive(true);
        }

        void HideError()
        {
            errorIcon.SetActive(false);
        }
    }
}
