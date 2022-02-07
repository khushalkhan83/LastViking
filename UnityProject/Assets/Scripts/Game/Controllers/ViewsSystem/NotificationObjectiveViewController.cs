using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class NotificationObjectiveViewControllerData : IDataViewController
    {
        public ObjectiveModel ObjectiveModel { get; }

        public NotificationObjectiveViewControllerData(ObjectiveModel objectiveModel) => ObjectiveModel = objectiveModel;
    }

    public class NotificationObjectiveViewController : ViewControllerBase<ObjectiveProcessView, NotificationObjectiveViewControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public ObjectivesNotificationModel ObjectivesNotificationModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }

        Coroutine ShowProcess { get; set; }

        protected override void Show()
        {
            Data.ObjectiveModel.OnProgress += OnProgressObjective;
            Data.ObjectiveModel.OnComplete += OnCompleteObjectiveHandler;
            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            if (Data.ObjectiveModel.IsComplete)
            {
                SetProgress(1);
                StartProcess(CompleteProcess());
            }
            else
            {
                SetProgress(Data.ObjectiveModel.Progress);
                UpdateLocalization();
                StartProcess(EndProcess());
            }
        }

        protected override void Hide()
        {
            Data.ObjectiveModel.OnComplete -= OnCompleteObjectiveHandler;
            Data.ObjectiveModel.OnProgress -= OnProgressObjective;
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
            StopAllCoroutines();
            ShowProcess = null;
        }

        private void OnChangeLanguageHandler() => UpdateLocalization();

        private void UpdateLocalization()
        {
            UpdateTextDescription();
            UpdateTextProgress();
        }

        private void OnCompleteObjectiveHandler(ObjectiveModel objectiveModel) => StartProcess(CompleteProcess());

        private void StartProcess(IEnumerator process)
        {
            if (ShowProcess != null)
            {
                StopCoroutine(ShowProcess);
            }

            ShowProcess = StartCoroutine(process);
        }

        private IEnumerator EndProcess()
        {
            yield return new WaitForSeconds(3f);
            NotificationContainerViewModel.EndCurrent();
            ObjectivesNotificationModel.Hide(ObjectivesModel.GetId(Data.ObjectiveModel));
        }

        private IEnumerator CompleteProcess()
        {
            View.PlayComplete();
            yield return new WaitForSeconds(0.16f);
            UpdateLocalization();
            yield return new WaitForSeconds(3.34f);
            NotificationContainerViewModel.EndCurrent();
            ObjectivesNotificationModel.Hide(ObjectivesModel.GetId(Data.ObjectiveModel));
        }

        private void OnProgressObjective(ObjectiveModel objectiveModel) => UpdateProgress();

        private void UpdateProgress()
        {
            UpdateTextProgress();
            SetProgress(Data.ObjectiveModel.Progress);
        }

        private void UpdateTextDescription() => View.SetDescriptionText(LocalizationModel.GetString(GetDescriptionKeyID()));

        private LocalizationKeyID GetDescriptionKeyID()
        {
            if (Data.ObjectiveModel.IsComplete)
            {
                return Data.ObjectiveModel.ObjectiveData.DescriptionCompleteKeyID;
            }

            return Data.ObjectiveModel.ObjectiveData.DescriptionKeyID;
        }

        private void UpdateTextProgress() => View.SetTimeText(GetTimeText());

        private string GetTimeText()
        {
            if (Data.ObjectiveModel.IsComplete)
            {
                return string.Empty;
            }
            else
            {
                var all = Data.ObjectiveModel.GetAllInt;
                if (all == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return $"{Data.ObjectiveModel.GetCurrentInt}/{all}";
                }
            }
        }

        private void SetProgress(float value)
        {
            View.SetSliderValue(value);
        }
    }
}
