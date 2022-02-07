using Core.Views;
using Game.Audio;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Core.Controllers
{
    public class WindowViewControllerBase<V> : ViewControllerBase<V> where V : IView
    {
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected override void Show()
        {
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
        }
        
        protected override void Hide()
        {
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
        }

        private void OnCloseViewHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }
    }

    public class WindowViewControllerBase<V,D> : ViewControllerBase<V, D> where V : IView
    {
        [Inject] public InputModel InputModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected override void Show()
        {
            InputModel.OnInput.AddListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
        }
        
        protected override void Hide()
        {
            InputModel.OnInput.RemoveListener(PlayerActions.UIMenu_B,OnCloseViewHandler);
        }

        private void OnCloseViewHandler()
        {
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }
    }
}
