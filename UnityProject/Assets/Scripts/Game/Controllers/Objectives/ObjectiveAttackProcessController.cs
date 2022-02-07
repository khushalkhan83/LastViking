using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using System.Collections;

namespace Game.Controllers
{
    public class ObjectiveAttackProcessController : IObjectiveAttackProcessController, IController
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public SheltersModel SheltersModel { get; private set; }
        [Inject] public SkeletonSpawnManager SkeletonSpawnManager { get; private set; }

        protected ObjectiveAttackProcessView ObjectiveAttackProcessView { get; set; }

        private bool IsHasShelter => SheltersModel.ShelterActive != ShelterModelID.None;
        private bool SessionSpawned => SkeletonSpawnManager.IsSessionStarted;

        void IController.Enable()
        {
            NotificationContainerViewModel.OnShow += OnShowNotificationHandler;
            NotificationContainerViewModel.OnHide += OnHideNotificationHandler;

            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;

            SkeletonSpawnManager.OnSessionStarted += OnSessionStarted;
            SkeletonSpawnManager.OnSessionCleared += OnSessionCleared;
            SkeletonSpawnManager.OnEnableSpawn += OnEnableSpawn;
            SkeletonSpawnManager.OnDisableSpawn += OnDisableSpawn;
        }

        void IController.Start()
        {
            UpdateViewVisible();
        }

        void IController.Disable()
        {
            NotificationContainerViewModel.OnShow -= OnShowNotificationHandler;
            NotificationContainerViewModel.OnHide -= OnHideNotificationHandler;
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            SkeletonSpawnManager.OnSessionStarted -= OnSessionStarted;
            SkeletonSpawnManager.OnSessionCleared -= OnSessionCleared;
            SkeletonSpawnManager.OnEnableSpawn -= OnEnableSpawn;
            SkeletonSpawnManager.OnDisableSpawn -= OnDisableSpawn;

            //  Костыль. Переделать так, чтобы корутин не было напрямую в конртроллерах
            if (!_destroyed)
            {
                SheltersModel.StopAllCoroutines();
            }
            HideView();
        }

        private bool _destroyed = false;
        private void OnDestroy() => _destroyed = true;

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();

        private bool IsCanShow => !PlayerHealthModel.IsDead && IsHasShelter && SessionSpawned;

        private void OnShowNotificationHandler()
        {
            if (NotificationContainerViewModel.StateIDLast == NotificationStateID.None)
            {
                ObjectiveAttackProcessView?.PlayMoveSecond();
            }
        }

        private void OnHideNotificationHandler()
        {
            if (NotificationContainerViewModel.StateIDNext == NotificationStateID.None)
            {
                ObjectiveAttackProcessView?.PlayMoveFirst();
            }
        }

        private void OnSessionStarted() => ShowView();
        private void OnSessionCleared() => HideView();
        private void OnEnableSpawn() => SheltersModel.StartCoroutine(WaitForUpdateViewVisible());
        private void OnDisableSpawn() => SheltersModel.StartCoroutine(WaitForUpdateViewVisible());

        private void UpdateViewVisible()
        {
            if (IsCanShow)
            {
                ShowView();
            }
            else
            {
                HideView();
            }
        }

        private IEnumerator WaitForUpdateViewVisible()
        {
            yield return null;
            UpdateViewVisible();
        }

        private void ShowView()
        {
            if (ObjectiveAttackProcessView == null)
            {
                ObjectiveAttackProcessView = ViewsSystem.Show<ObjectiveAttackProcessView>(ViewConfigID.ObjectiveAttackProcess);
                if (NotificationContainerViewModel.IsHasCurrent)
                {
                    ObjectiveAttackProcessView.PlayMoveSecond();
                }
                ObjectiveAttackProcessView.OnHide += OnHideHandler;
            }
        }

        private void HideView()
        {
            if (ObjectiveAttackProcessView != null)
            {
                ViewsSystem.Hide(ObjectiveAttackProcessView);
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            ObjectiveAttackProcessView = null;
        }
    }
}
