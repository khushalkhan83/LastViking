using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class DiscordController : IDiscordController, IController
    {
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public DiscordModel DiscordModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private DiscordButtonView DiscordButtonView { get; set; }

        private int _enableProcessCoroutineId = -1;
        private int _timerProcessCoroutineId = -1;

        void IController.Enable()
        {
            _enableProcessCoroutineId = CoroutineModel.InitCoroutine(EnableProcess());
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalPlayerHandler;
            HideDiscordButton();
            CoroutineModel.BreakeCoroutine(_enableProcessCoroutineId);
            CoroutineModel.BreakeCoroutine(_timerProcessCoroutineId);
        }

        private IEnumerator EnableProcess()
        {
            yield return null;
            if (PlayerDeathModel.DeathCount >= DiscordModel.DeathCount)
            {
                ShowDiscordButton();
            }
        }

        private void OnRevivalPlayerHandler()
        {
            ShowDiscordButton();
        }

        private void ShowDiscordButton()
        {
            if (DiscordButtonView == null)
            {
                DiscordButtonView = ViewsSystem.Show<DiscordButtonView>(ViewConfigID.DiscordButton);
                _timerProcessCoroutineId = CoroutineModel.InitCoroutine(TimerProcess(DiscordModel.Duration, HideDiscordButton));
            }
        }

        private void HideDiscordButton()
        {
            if (DiscordButtonView != null)
            {
                ViewsSystem.Hide(DiscordButtonView);
                DiscordButtonView = null;
            }
        }

        private IEnumerator TimerProcess(float duration, Action callback)
        {
            yield return new WaitForSeconds(duration);
            callback();
        }
    }
}
