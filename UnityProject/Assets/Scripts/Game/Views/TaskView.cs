using System;
using Core.Views;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    [RequireComponent(typeof(CanvasGroup))]
    public class TaskView : ViewBase
    {
       #region Data
        #pragma warning disable 0649
        [SerializeField] private Text message;
        [SerializeField] private Text countField;
        [SerializeField] protected Image _amountImage;
        [SerializeField] protected GameObject _fillObject;
        [SerializeField] private Image _icon;
        #pragma warning restore 0649
        #endregion

        public string Message {get => message.text; set => message.text = value;}
        public string CountField {get => countField.text; set => countField.text = value;}

        [Header("Animation")]
        [SerializeField] private float animationTime = 0.3f;
        [SerializeField] private float fillAnimationTime = 0.05f;
        [SerializeField] private Ease ease = Ease.InQuad;
        [SerializeField] private float yAxisAnimationShift = 100f;

        private CanvasGroup canvas;
        private float? defaultYPosition;
        private Sprite defaultIcon;

        public bool IsShowing {get;set;}

        #region MonoBehaviour
        private void OnEnable() 
        {
            IsShowing = false;
        }
        private void Awake()
        {
            canvas = GetComponent<CanvasGroup>();
            defaultIcon = _icon.sprite;
        }
        #endregion

        public void SetFillAmount(float amount, bool animate = true)
        {
            if(animate)
            {
                _amountImage.DOFillAmount(amount, fillAnimationTime).SetEase(ease);
            }
            else
                _amountImage.fillAmount = amount;
        }
        public void ShowFill(bool show)
        {
            _fillObject.SetActive(show);
        }

        public void SetIcon(Sprite icon) => _icon.sprite = icon == null ? defaultIcon : icon;

        // TODO: move animation to base class. TODO: handle scenario with screen size changed

        [Button]
        private void Test_PlayShowAnimation() => PlayShowAnimation(null);

        public void PlayShowAnimation(Action callback = null)
        {
            var defaultAnimationTime = animationTime;
            if(oneShotAnimationTime.HasValue)
            {
                animationTime = oneShotAnimationTime.Value;
            }

            StopAnimations();
            // canvas.DOFade(0,0);
            canvas.alpha = 0;
            canvas.DOFade(1,animationTime).SetEase(ease);


            if(defaultYPosition == null) defaultYPosition = transform.position.y;

            transform.DOMoveY(defaultYPosition.Value + yAxisAnimationShift,0);
            transform.DOMoveY(defaultYPosition.Value,animationTime).OnComplete(() =>
                { 
                    IsShowing = true;
                    callback?.Invoke();
                });


            oneShotAnimationTime = null;
            animationTime = defaultAnimationTime;
        }

        private float? oneShotAnimationTime;

        public void SetAnimationTimeOnce(float time)
        {
            if(time <= 0) return;

            oneShotAnimationTime = time;
        }

        [Button]
        private void TestPlay() => PlayHideAnimation(null);

        public void PlayHideAnimation(Action callback = null)
        {
            IsShowing = false;
            StopAnimations();
            canvas.DOFade(1,0);
            canvas.DOFade(0,animationTime).SetEase(Ease.InQuint);

            if(defaultYPosition == null) defaultYPosition = transform.position.y;

            transform.DOMoveY(defaultYPosition.Value ,0);
            transform.DOMoveY(defaultYPosition.Value + yAxisAnimationShift,animationTime).SetEase(Ease.InQuint).OnComplete(() => callback?.Invoke());
        }

        private void StopAnimations()
        {
            DOTween.Kill(canvas);
            DOTween.Kill(transform);
        }
    }
}
