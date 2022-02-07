using Core;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_Template : TutorialStepBase
    {
        // [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        public TutorialStep_Template(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }
        
        public override void OnStart()
        {
            throw new System.NotImplementedException();
        }
        public override void OnEnd()
        {
            throw new System.NotImplementedException();
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }
    }
}