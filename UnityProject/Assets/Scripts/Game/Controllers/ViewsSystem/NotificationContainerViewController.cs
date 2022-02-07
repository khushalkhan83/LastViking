using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class NotificationContainerViewController : ViewControllerBase<NotificationContainerView>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        private IView CurrentView { get; set; }

        protected override void Show()
        {
            NotificationContainerViewModel.OnShow += OnShowNotificationHandler;
            NotificationContainerViewModel.OnHide += OnHideNotificationHandler;

            if (NotificationContainerViewModel.DataCurrent != null)
            {
                OnShowNotificationHandler();
            }
        }

        protected override void Hide()
        {
            NotificationContainerViewModel.OnShow -= OnShowNotificationHandler;
            NotificationContainerViewModel.OnHide -= OnHideNotificationHandler;

            if (CurrentView != null)
            {
                ViewsSystem.Hide(CurrentView);
            }
        }

        private void OnShowNotificationHandler()
        {
            var data = NotificationContainerViewModel.DataCurrent;
            var view = ViewsSystem.Show(data.ViewConfigID, View.Container, data.DataViewController);
            if (view is INotification notificationView)
            {
                if (NotificationContainerViewModel.StateIDCurrent == NotificationStateID.Cache)
                {
                    notificationView.SetAsFirst();
                    notificationView.PlayShow();
                }
                else
                {
                    notificationView.SetAsLast();
                    notificationView.PlayShowTop();
                }
            }

            CurrentView = view;
        }

        private void OnHideNotificationHandler()
        {
            StartCoroutine(HideCurrentView(CurrentView, NotificationContainerViewModel.StateIDNext));
            CurrentView = null;
        }

        private IEnumerator HideCurrentView(IView view, NotificationStateID nextStateID)
        {
            if (view is INotification notificationView)
            {
                switch (nextStateID)
                {
                    case NotificationStateID.None:
                    case NotificationStateID.First:
                    case NotificationStateID.Cache:
                        notificationView.PlayHideTop();
                        break;
                }
            }
            yield return new WaitForSeconds(1f);
            ViewsSystem.Hide(view);
        }
    }
}
