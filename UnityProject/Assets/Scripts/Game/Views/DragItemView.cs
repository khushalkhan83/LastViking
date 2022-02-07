using Core.Views;
using DG.Tweening;
using NaughtyAttributes;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class DragItemView : ViewBase
    {
        [Header("General")]
        [SerializeField] private Image image;

        [Header("Drag related")]
        [SerializeField] private float duration;
        [SerializeField] private Transform dragStartPoint;
        [SerializeField] private Transform dragEndPoint;

        [Header("Click related")]
        [SerializeField] private float timeToClick = 0.3f;
        [SerializeField] private float restTimeAfterClick = 0.7f;
        [SerializeField] private Image circleImage;
        [SerializeField] private Transform clickPoint;
        [SerializeField] private Transform defaultClickPoint;


        private Vector3 defaultImageScale;
        private Vector3 defaultCircleScale;


        private Action OnDragAnimationEnded;
        private Action OnClickAnimationEnded;

        #region MonoBehaviour
        private void Awake()
        {
            defaultImageScale = image.transform.localScale;
            defaultCircleScale = circleImage.transform.localScale;
        }

        #endregion

        public void PlayDragAnimation(Transform pointA, Transform pointB)
        {
            this.dragStartPoint = pointA;
            this.dragEndPoint = pointB;

            image.transform.position = pointA.transform.position;
            PlayDragAnimation_Internal();
        }

        [Button]
        public void PlayDragAnimation_Internal()
        {
            UnSubscribe();
            OnDragAnimationEnded += PlayDragAnimation_Internal;

            DOTween.Kill(image);
            DOTween.Kill(image.transform);
            DOTween.Kill(circleImage);
            DOTween.Kill(circleImage.transform);

            circleImage.DOFade(0,0);

            int startAnimationTime = 1;
            image.transform.DOScale(defaultImageScale + Vector3.one * 0.2f, 0);
            image.transform.DOScale(defaultImageScale, startAnimationTime);
            image.DOFade(1, startAnimationTime).OnComplete(() =>
            {
                image.transform.DOMove(dragEndPoint.transform.position, duration).OnComplete(() =>
                {
                    image.DOFade(0, 0.1f);
                    image.transform.DOMove(dragEndPoint.transform.position, 0.1f).OnComplete(() =>
                    {
                        image.transform.DOMove(dragStartPoint.transform.position, 1).OnComplete(() =>
                        {
                            OnDragAnimationEnded?.Invoke();
                        });
                    });
                });
            });
        }

        public void PlayClickAnimation(Transform position = null)
        {
            clickPoint = position == null ? defaultClickPoint : position;

            PlayClickAnimation_Internal();
        }

        [Button] 
        public void PlayClickAnimation_Internal()
        {
            UnSubscribe();
            DOTween.Kill(image);
            DOTween.Kill(image.transform);
            DOTween.Kill(circleImage);
            DOTween.Kill(circleImage.transform);

            OnClickAnimationEnded += PlayClickAnimation_Internal;

            image.DOFade(1,0);
            image.transform.DOMove(clickPoint.position,0);
            circleImage.transform.DOScale(0,0);
            circleImage.DOFade(1,0);

            image.transform.DOScale(defaultImageScale + Vector3.one * 0.2f, 0);
            image.transform.DOScale(defaultImageScale, timeToClick).OnComplete(() =>
            {
                circleImage.transform.DOScale(defaultCircleScale,restTimeAfterClick);
                circleImage.DOFade(0,restTimeAfterClick);
                image.transform.DOScale(defaultImageScale + Vector3.one * 0.2f,restTimeAfterClick).OnComplete(() => {
                    OnClickAnimationEnded?.Invoke();
                });
            });
        }

        private void UnSubscribe()
        {
            OnDragAnimationEnded -= PlayDragAnimation_Internal;
            OnClickAnimationEnded -= PlayClickAnimation_Internal;
        }
    }
}
