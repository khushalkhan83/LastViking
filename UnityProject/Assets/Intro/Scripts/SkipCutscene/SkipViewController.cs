using UnityEngine;
using UnityEngine.Events;

namespace Game.Intro
{
    public class SkipViewController : MonoBehaviour
    {
        #region Data
    #pragma warning disable 0649
        [SerializeField] private SkipView view;
        [SerializeField] private float waitTime = 2.0f;
        [SerializeField] private UnityEvent onSkip;
    #pragma warning restore 0649
        #endregion

        private float timer = 0.0f;

        private bool isPressed;

        private bool isDone;

        #region MonoBehaviour
        private void OnEnable()
        {
            view._OnPointerDown += OnPointerDown;
            view._OnPointerUp += OnPointerUp;

            ResetView();
        }

        private void OnDisable()
        {
            view._OnPointerDown -= OnPointerDown;
            view._OnPointerUp -= OnPointerUp;
        }

        private void Update()
        {
            if(isPressed)
            {
                timer += Time.deltaTime;
            
                var fill = CalculateFillAmount();
                view.SetFill(fill);

                if(fill >= 1)
                {
                    isDone = true;
                    onSkip?.Invoke();
                }
            }
        }
        #endregion

        private void OnPointerDown()
        {
            isPressed = true;

            view.ShowTitle();
        }

        private void OnPointerUp()
        {
            isPressed = false;

            ResetView();
        }

        private void ResetView()
        {
            timer = 0;
            isDone = false;
            view.SetFill(0);
            view.HideTitle();
        }

        private float CalculateFillAmount()
        {
            var fillAmount = timer/ waitTime;
            return fillAmount;
        }
    }
}
