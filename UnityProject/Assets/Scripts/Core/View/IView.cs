using Game.Views;
using System;

namespace Core.Views
{
    public interface IView
    {
        void Show();
        void Hide();

        event Action<IView> OnShow;
        event Action<IView> OnHide;
    }
}
