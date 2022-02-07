using Core;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_Wrapper_Teamplate : TutorialStepBase
    {
        public TutorialStep_Wrapper_Teamplate(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        private ITutorialStep step;

        public override void OnStart()
        {
            Init();
            step.Enter();
        }
        public override void OnEnd()
        {
            step.Exit();
            step = null;
        }

        private void Init()
        {
            // GameObject enemy = null;
            // step = new TutorialStep_KillEnemy(CheckConditions,enemy,"Убей волка");
            InjectionSystem.Inject(step);
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }
    }
}