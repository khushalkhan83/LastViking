using System;
using System.Collections;
using Core;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class ActivateObjectivesViewController : ViewEnableController<ObjectivesView>, IActivateObjectivesViewController
    {
        [Inject] public ObjectivesViewModel ViewModel { get; private set; }
        [Inject] public LocalNotificationsModel LocalNotificationsModel { get; private set; }
        [Inject] public ViewsStateModel ViewsStateModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        

        private const float k_canShowCheckTime = 2f;
        private bool redirected;
        private bool redirectRequest;
        private int coroutine = -1;

        public override ViewConfigID ViewConfigID => ViewConfigID.Objectives;

        public override bool IsCanShow
        {
            get
            {
                return redirectRequest
                    && redirected == false
                    && !ViewsSystem.IsShow(ViewConfigID)
                    && !ViewsStateModel.WindowOrPopupOpened()
                    && !CinematicModel.CinematicStarted
                    && TutorialModel.IsPostTutorialCompleated;
            }
        }

        public override void Enable()
        {
            LocalNotificationsModel.OnGameStartedFromNotification += OnGameStartedFromNotification;
            HandleGameStartedFromNotification();
        }
        public override void Disable()
        {
            CoroutineModel.BreakeCoroutine(coroutine);

            LocalNotificationsModel.OnGameStartedFromNotification -= OnGameStartedFromNotification;
            Hide();
        }

        protected override void OnShow()
        {
            redirected = true;
            ViewModel.RedirectedFromPushNotification();
        }

        public override void Start() { }

        private void OnGameStartedFromNotification(LocalNotificationID notificationID)
        {
            if(redirectRequest == false)
            {
                HandleGameStartedFromNotification();
            }
        }

        private void HandleGameStartedFromNotification()
        {
            if(redirected) return;

            var gameInitNotification = LocalNotificationsModel.GameOpenedWithNotification;
            if(gameInitNotification == null) return;

            if(gameInitNotification.Value == LocalNotificationID.Objective)
            {
                redirectRequest = true;
                coroutine = CoroutineModel.InitCoroutine(WaitForCanShowView(() => UpdateViewVisible()));
            }
        }

        // TODO: remove duclicate code from StarterPackController
        private IEnumerator WaitForCanShowView(Action onCanShow = null)
        {
            while(true)
            {
                yield return new WaitForSeconds(k_canShowCheckTime);
                if (IsCanShow)
                {
                    onCanShow?.Invoke();
                    break;
                }
            }
        }
    }
}