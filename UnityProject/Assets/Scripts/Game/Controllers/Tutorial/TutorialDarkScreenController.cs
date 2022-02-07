using System;
using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TutorialDarkScreenController : ITutorialDarkScreenController, IController
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public TutorialDarkScreenModel TutorialDarkScreenModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }

        public IView View { get; private set; }

        private bool IsCanShow => TutorialModel.Step == TutorialDarkScreenModel.StepToShowScreen;

        void IController.Enable()
        {
            StorageModel.TryProcessing(TutorialDarkScreenModel.Data);

            if (!TutorialDarkScreenModel.HasShown)
            {
                PlayerDeathModel.OnRevival += OnRevivalHandler;
                PlayerDeathModel.OnRevivalPrelim += OnRevivalHandler;
            }
        }

        void IController.Start()
        {
            if (!TutorialDarkScreenModel.HasShown)
            {
                if (!TutorialModel.IsStart)
                {
                    TutorialModel.OnStart += OnStartTutorialHandler;
                }
                else
                {
                    if (!PlayerHealthModel.IsDead)
                    {
                        if (IsCanShow)
                        {
                            OpenView();
                        }
                    }
                }
            }
        }

        void IController.Disable()
        {
            CloseView();
            TutorialModel.OnStart -= OnStartTutorialHandler;
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalHandler;
        }

        private void OnRevivalHandler()
        {
            if (!TutorialModel.IsStart)
            {
                TutorialModel.OnStart += OnStartTutorialHandler;
            }
            else
            {
                if (IsCanShow)
                {
                    OpenView();
                }
            }
        }

        private void OnStartTutorialHandler() => OpenView();

        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<TutorialDarkView>(ViewConfigID.TutorialDark);
                View.OnHide += OnHideHandler;
            }
        }

        private void CloseView()
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

            OnShown();
        }

        private void OnShown()
        {
            TutorialDarkScreenModel.HasShown = true;
            TutorialModel.OnStart -= OnStartTutorialHandler;
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalHandler;
        }
    }
}
