using System;
using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{

    public class TutorialEvent
    {
        public TutorialEvent(string Name)
        {
            name = Name;
        }

        public event Action OnFired;
        private bool fiared = false;
        public readonly string name;

        public void Fire()
        {
            if(fiared) return;
            
            fiared = true;
            OnFired?.Invoke();
        }
    }

    public abstract class TutorialStepBase : ITutorialStep
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        // [Inject] public TaskMessageViewModel TaskMessageViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public InjectionSystem InjectionSystem { get; private set; }
        [Inject] public TaskViewModel TaskViewModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        public bool IsSubStep { get; }
        public Action OnCompleatedAsSubStep;
        private TutorialEvent StepStartedEvent {get;}

        public TutorialStepBase(TutorialEvent StepStartedEvent, Action OnCompleatedAsSubStep)
        {
            IsSubStep = true;
            this.OnCompleatedAsSubStep = OnCompleatedAsSubStep;
            this.StepStartedEvent = StepStartedEvent;
        }

        public TutorialStepBase(TutorialEvent StepStartedEvent)
        {
            IsSubStep = false;
            this.StepStartedEvent = StepStartedEvent;
        }
        
        public void Enter()
        {
            OnStart();
            StepStartedEvent?.Fire();
        }

        public void Exit()
        {
            OnEnd();
        }

        public abstract void OnStart();

        public abstract void OnEnd();


        protected void TutorialNextStep()
        {
            if(IsSubStep)
            {
                OnEnd();
                OnCompleatedAsSubStep?.Invoke();
            }
            else
            {
                TutorialModel.SetNextStep();
            }
        }

        // protected void ShowTaskMessage(LocalizationKeyID messageKeyID)
        // {
        //     var message = LocalizationModel.GetString(messageKeyID);
        //     // TaskMessageViewModel.SetMessage(message);
        //     // TaskMessageViewModel.SetShow(true);
        // }

        // protected void HideTaskMessage()
        // {
        //     // TaskMessageViewModel.SetMessage(string.Empty);
        //     // TaskMessageViewModel.SetShow(false);
        // }


        protected void ShowTaskMessage(bool show, string message = "", Sprite icon = null)
        {
            if (show)
            {
                TaskViewModel.SetShow(true);
                TaskViewModel.SetMessage(message);
                TaskViewModel.SetIcon(icon);
            }
            else
            {
                TaskViewModel.SetShow(false);
                TaskViewModel.SetIcon(null);
                // TaskViewModel.SetMessage(string.Empty);
            }
        }

        protected void ShowTaskFill(bool show)
        {
            TaskViewModel.SetShowFill(show);
        }

        protected void SetTaskFillValue(float value)
        {
            TaskViewModel.SetFillAmount(value);
        }

        protected void SetTaskFillValue(int minValue, int maxValue)
        {
            string extraText = $"{minValue}/{maxValue}";
            TaskViewModel.SetCount(extraText);
            TaskViewModel.SetFillAmount((float)minValue/(float)maxValue);
        }


        protected void EnsureEnoughItem(string itemName, int neededAmount)
        {
            var count = InventoryOperationsModel.GetItemCount(itemName);

            if (neededAmount > count)
            {
                var dif = neededAmount - count;
                InventoryOperationsModel.AddItemToPlayer(itemName, dif);
            }
        }
    }
}
