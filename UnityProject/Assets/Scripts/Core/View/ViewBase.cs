using System;
using UnityEngine;

namespace Core.Views
{
    public class ViewBase : MonoBehaviour, IView
    {
        public event Action<IView> OnShow;
        public event Action<IView> OnHide;

        void IView.Show() => OnShow?.Invoke(this);
        void IView.Hide() => OnHide?.Invoke(this);
    }
}
