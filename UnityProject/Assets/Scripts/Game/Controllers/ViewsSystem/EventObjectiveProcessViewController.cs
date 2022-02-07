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
    public class EventObjectiveProcessViewControllerData : IDataViewController
    {
        public ObjectiveModel ObjectiveModel { get; }

        public EventObjectiveProcessViewControllerData(ObjectiveModel objectiveModel) => ObjectiveModel = objectiveModel;
    }

    public class EventObjectiveProcessViewController : ViewControllerBase<ObjectiveProcessView, EventObjectiveProcessViewControllerData>
    {
        [Inject] public NotificationContainerViewModel NotificationContainerViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public QuestNotificationsModel QuestNotificationsModel { get; private set; }
        [Inject] public ShelterAttackModeModel ShelterAttackModeModel { get; private set; }

        // temp fix to work with quest notifications and shelter attack mode messages (not driven by NotificationContainerViewModel)
        private bool IsTombObjective => Data.ObjectiveModel.ObjectiveID == ObjectiveID.TombInteract;

        protected override void Show()
        {
            Data.ObjectiveModel.OnProgress += OnProgressObjective;
            Data.ObjectiveModel.OnComplete += OnCompleteObjectiveHandler;
            LocalizationModel.OnChangeLanguage += OnChangeLanguageHandler;

            QuestNotificationsModel.OnShow += Refresh;
            QuestNotificationsModel.OnHide += Refresh;
            
            ShelterAttackModeModel.OnAttackModeActiveChanged += Refresh;

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

            // temp fix to work with quest notifications and shelter attack mode messages (not driven by NotificationContainerViewModel)
            messageDisplayed = true;
            StartCoroutine(DoActionAfterTime(3, () => Refresh()));

        }

        #region // temp fix to work with quest notifications and shelter attack mode messages (not driven by NotificationContainerViewModel)

        private void Refresh()
        {
            if(!IsTombObjective) return;
            
            if(NeedHide())
            {
                if(messageDisplayed)
                    View.PlayHideTop();
                else
                    View.PlayHidden();

                messageDisplayed = false;
            }
            else
            {
                View.PlayShow();
                messageDisplayed = true;
            }
        }
        
        private bool messageDisplayed;

        private bool NeedHide()
        {
            if(!IsTombObjective) return true;

            bool hide = QuestNotificationsModel.IsShown || ShelterAttackModeModel.AttackModeActive;

            return hide;
        }

        private IEnumerator DoActionAfterTime(float time, Action action)
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }
   
        #endregion


        protected override void Hide()
        {
            Data.ObjectiveModel.OnComplete -= OnCompleteObjectiveHandler;
            Data.ObjectiveModel.OnProgress -= OnProgressObjective;
            LocalizationModel.OnChangeLanguage -= OnChangeLanguageHandler;

            QuestNotificationsModel.OnShow -= Refresh;
            QuestNotificationsModel.OnHide -= Refresh;
            ShelterAttackModeModel.OnAttackModeActiveChanged -= Refresh;

            StopAllCoroutines();
        }

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
