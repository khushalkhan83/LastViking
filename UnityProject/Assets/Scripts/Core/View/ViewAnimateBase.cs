using System;
using UnityEngine;

namespace Core.Views
{
    public class ViewAnimateBase : MonoBehaviour, IView
    {
        public int TriggerIdShow => Animator.StringToHash("Show");
        public int TriggerIdHide => Animator.StringToHash("Hide");

        #region Data
#pragma warning disable 0649

        [SerializeField] private Animator _animator;

#pragma warning restore 0649
        #endregion

        protected Animator Animator => _animator;

        public event Action<IView> OnShow;
        public event Action<IView> OnHide;

        void IView.Show()
        {
            if (Animator != null)
            {
                Animator.SetTrigger(TriggerIdShow);
            }
            else
            {
                OnShow?.Invoke(this);
            }
        }

        void IView.Hide()
        {
            if (Animator != null)
            {
                Animator.SetTrigger(TriggerIdHide);
            }
            else
            {
                OnHide?.Invoke(this);
            }
        }

        //Animation event
        private void _Show() => OnShow?.Invoke(this);

        //Animation event
        private void _Hide() => OnHide?.Invoke(this);
    }
}
