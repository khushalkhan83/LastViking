using Core.Views;
using System;

namespace Game.Views
{
    public class AddCellWatchView : ViewBase
    {
        //UI
        public event Action OnClick;
        public void ActionClick() => OnClick?.Invoke();
    }
}
