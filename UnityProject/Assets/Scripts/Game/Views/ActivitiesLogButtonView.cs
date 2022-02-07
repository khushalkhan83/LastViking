using Core.Views;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Views
{
    public class ActivitiesLogButtonView : ViewBase
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Button button;
        [SerializeField] private Text counter;
        [SerializeField] private GameObject counterHolder;
        [SerializeField] private Animator animator;
#pragma warning restore 0649
        #endregion

        public event Action OnClick;

        #region MonoBehaviour
        private void OnEnable()
        {
            button.onClick.AddListener(() => OnClick?.Invoke());
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }
        #endregion

        public string Counter {set => counter.text = value;}
        public void ShowCounter(bool show) => counterHolder.gameObject.SetActive(show);

        public void SetPlayPulseAnimation(bool play)
        {
            animator.enabled = play;

            if(play == false)
            {
                animator.gameObject.transform.localScale = Vector3.one;
            }
        }
    }
}
