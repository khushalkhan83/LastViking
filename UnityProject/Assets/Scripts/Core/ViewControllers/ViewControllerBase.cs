using Core.Views;
using UnityEngine;

namespace Core.Controllers
{
    public abstract class ViewControllerBase<V> : MonoBehaviour, IViewController where V : IView
    {
        protected V View { get; private set; }

        void IViewController.Enable(object view)
        {
            View = (V)view;
            Show();
        }

        void IViewController.Disable()
        {
            Hide();
        }

        protected abstract void Show();
        protected abstract void Hide();
    }

    public abstract class ViewControllerBase<V, D> : ViewControllerBase<V>, IViewControllerData where V : IView
    {
        protected D Data { get; private set; }

        void IViewControllerData.Set(object data) => Data = (D)data;
    }
}
