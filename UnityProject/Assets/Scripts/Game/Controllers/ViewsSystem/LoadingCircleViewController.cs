using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class LoadingCircleViewController : ViewControllerBase<LoadingCircleView>
    {
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LoadingCircleViewModel LoadingCircleViewModel { get; private set; }
        [Inject] public NetworkModel NetworkModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }

        protected Coroutine ShowProcess { get; set; }

        protected override void Show()
        {
            View.OnClose += OnCloseHandler;
            View.SetVisibleCloseButton(false);
            ShowProcess = StartCoroutine(ShowButton());
            GameTimeModel.ScaleSave();
            GameTimeModel.ScaleStop();
        }

        protected override void Hide()
        {
            GameTimeModel.ScaleRestore();

            View.OnClose -= OnCloseHandler;
            if (ShowProcess != null)
            {
                StopCoroutine(ShowProcess);
            }
        }

        private void OnCloseHandler()
        {
            NetworkModel.UpdateInternetConnectionStatus();
            AudioSystem.PlayOnce(AudioID.Button);
            ViewsSystem.Hide(View);
        }

        private IEnumerator ShowButton()
        {
            yield return new WaitForSecondsRealtime(LoadingCircleViewModel.ButtonAppearTime);
            View.SetVisibleCloseButton(true);
        }
    }
}
