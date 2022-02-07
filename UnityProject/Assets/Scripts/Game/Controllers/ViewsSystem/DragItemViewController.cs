using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;

namespace Game.Controllers
{
    public class DragItemViewController : ViewControllerBase<DragItemView>
    {
        [Inject] public DragItemViewModel ViewModel { get; private set; }

        protected override void Show() 
        {
            ViewModel.OnDataChanged += UpdateView;
            UpdateView();
        }

        protected override void Hide() 
        {
            ViewModel.OnDataChanged -= UpdateView;
        }

        private void UpdateView()
        {
            if(ViewModel.Animation == DragItemViewModel.AnimationType.Click)
            {
                View.PlayClickAnimation(ViewModel.ClickPosition);
            }
            else
            {
                View.PlayDragAnimation(ViewModel.From, ViewModel.To);
            }
        }
    }
}
