using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class ObjectiveProcessViewControllerData : IDataViewController
    {
        public ObjectiveModel ObjectiveModel { get; }

        public ObjectiveProcessViewControllerData(ObjectiveModel objectiveModel) => ObjectiveModel = objectiveModel;
    }

    public class TutorialObjectiveProcessViewController : ViewControllerBase<ObjectiveProcessView, ObjectiveProcessViewControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        protected override void Show()
        {
            Data.ObjectiveModel.OnProgress += OnProgressObjective;
            Data.ObjectiveModel.OnComplete += OnCompleteObjectiveHandler;
            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;
            TutorialModel.OnSkipTutorial += OnSkipTutorialHandler;

            if (Data.ObjectiveModel.IsComplete)
            {
                SetProgress(1);
                StartCompleteProcess();
            }
            else
            {
                SetProgress(Data.ObjectiveModel.Progress);
                UpdateLocalization();
            }
        }

        protected override void Hide()
        {
            Data.ObjectiveModel.OnComplete -= OnCompleteObjectiveHandler;
            Data.ObjectiveModel.OnProgress -= OnProgressObjective;
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;
            TutorialModel.OnSkipTutorial -= OnSkipTutorialHandler;

            if(this != null) StopAllCoroutines();
        }

        private void OnSkipTutorialHandler() => StartCompleteProcess();
        private void OnChangeLanguageHandler() => UpdateLocalization();

        private void UpdateLocalization()
        {
            UpdateTextDescription();
            UpdateTextProgress();
        }

        private void OnCompleteObjectiveHandler(ObjectiveModel objectiveModel) => StartCompleteProcess();

        private void StartCompleteProcess() => StartCoroutine(CompleteProcess());

        private IEnumerator CompleteProcess()
        {
            View.PlayComplete();
            yield return new WaitForSeconds(0.16f);
            UpdateLocalization();
            yield return new WaitForSeconds(3.34f);
            NotificationContainerViewModel.EndCurrent();
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
