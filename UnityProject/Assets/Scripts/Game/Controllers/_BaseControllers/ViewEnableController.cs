using Core;
using Core.Controllers;
using Core.Views;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public abstract class ViewEnableController<V> : IController where V : Component, IView
    {
        [Inject] public ViewsSystem ViewsSystem { get; protected set; }

        public abstract void Enable();
        public virtual void Start() {}
        public abstract void Disable();

        public abstract ViewConfigID ViewConfigID { get; }
        public abstract bool IsCanShow { get; }
        public virtual IDataViewController Data { get; } = null;

        protected virtual void OnShow() { }
        protected virtual void OnHide() { }

        protected virtual void OnShowView() { }
        protected virtual void OnHideView() { }
        
        protected V View { get; set; }

        protected void UpdateViewVisible()
        {
            if (IsCanShow)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        protected void Show()
        {
            ShowView();
            OnShow();
            if(View != null)
            {
                OnShowView();
            }
        }

        protected void Hide()
        {
            bool viewWasNotNull = View != null;
            HideView();
            OnHide();
            if(viewWasNotNull && View == null)
            {
                OnHideView();
            }
        }

        private void ShowView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<V>(ViewConfigID, Data);
                View.OnHide += OnHideHandler;
            }
        }

        private void HideView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }
    }
}
