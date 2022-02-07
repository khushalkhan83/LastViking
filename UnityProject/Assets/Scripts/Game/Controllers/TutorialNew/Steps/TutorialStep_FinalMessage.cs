using System;
using System.Collections;
using Core;
using Game.Models;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_FinalMessage : TutorialStepBase
    {
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private int coroutineIndex = -1;
        private const float messageDuration = 6;

        public TutorialStep_FinalMessage(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        public override void OnStart()
        {
            ShowTaskMessage(true,"Use your new skills to survive for as long as you can!");
            coroutineIndex = CoroutineModel.InitCoroutine(CDoActionAfterSeconds(() => CheckConditions(),messageDuration));
        }
        public override void OnEnd()
        {
            CoroutineModel.BreakeCoroutine(coroutineIndex);
            ShowTaskMessage(false);
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }


        // TODO: Code duplicate here and in other classes. Move to CoroutineModel
        private IEnumerator CDoActionAfterSeconds(Action action, float seconds)
        {
            yield return new WaitForSeconds(seconds);
            action?.Invoke();
        }
    }
}