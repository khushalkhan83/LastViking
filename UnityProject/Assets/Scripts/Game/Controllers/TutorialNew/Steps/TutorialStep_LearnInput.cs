using Core;
using Game.Models;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_LearnInput : TutorialStepBase
    {
        public TutorialStep_LearnInput(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        [Inject] public LearnInputViewModel LearnInputViewModel { get; private set; }
        
        public override void OnStart()
        {
            LearnInputViewModel.SetShow(true);
            LearnInputViewModel.OnFinish += CheckConditions;
        }
        public override void OnEnd()
        {
            LearnInputViewModel.SetShow(false);
            LearnInputViewModel.OnFinish -= CheckConditions;
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }
    }
}