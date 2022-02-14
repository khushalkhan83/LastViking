using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using DG.Tweening;

namespace UI_Template.Animations
{
    [RequireComponent(typeof(CanvasGroup))]
    public class OpenAnimation : MonoBehaviour
    {
        private CanvasGroup group;
        [SerializeField] private float startScale = 0.8f;
        [SerializeField] private float startAlpha = 0.8f;
        [SerializeField] private float aniamtionDuration = 0.3f;
        [SerializeField] private Ease easeType = Ease.InOutCubic;

        private float originalScale = 1f;


        #region MonoBehaviour
        private void Awake()
        {
            originalScale = transform.localScale.x;
            group = GetComponent<CanvasGroup>();
        }

        private void OnEnable()
        {
            PlayAnimation();
        }
        #endregion

        [Button]
        public void PlayAnimation()
        {
            StopAllCoroutines();
            StartCoroutine(Animation());
        }

        private IEnumerator Animation()
        {
            transform.DOScale(originalScale*startScale,0).SetUpdate(true);
            group.DOFade(startAlpha,0).SetUpdate(true);

            yield return new WaitForEndOfFrame();

            transform.DOScale(originalScale,aniamtionDuration).SetEase(easeType).SetUpdate(true);

            group.DOFade(1,aniamtionDuration).SetEase(easeType).SetUpdate(true);
        }
    }
}