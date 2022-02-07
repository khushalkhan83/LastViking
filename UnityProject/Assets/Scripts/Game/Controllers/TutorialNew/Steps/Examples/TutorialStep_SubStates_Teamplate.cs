using Core;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_SubStates_Teamplate : TutorialStepBase
    {
        public TutorialStep_SubStates_Teamplate(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        protected TutorialStepsStateMachine stateMachine = new TutorialStepsStateMachine();

        private void Init()
        {
            // var sceneContext = GameObject.FindObjectOfType<SimpleTutorialController>();
            // rabbit = sceneContext.rabbit;
        }

        public override void OnStart()
        {
            Init();
            DoSomething_1_Task();
        }
        
        public override void OnEnd()
        {
            ShowTaskMessage(false);
        }

        // sub step 1
        private void DoSomething_1_Task()
        {
            // ITutorialStep subStep = new TutorialStep_KillEnemy(StepStartedEvent, onCompleatedAsSubStep: DoSomething_2_Task, rabbit,killRabbitMessage);
            // SetState(subStep);
        }

        // sub step 2
        private void DoSomething_2_Task()
        {
            // ITutorialStep subStep = new TutorialStep_TakeItem(StepStartedEvent, OnCompleatedAsSubStep: CheckConditions,targetItemID,tokenConfigID);
            // SetState(subStep);
        }

        // final step
        private void CheckConditions()
        {
            bool nextStep = true;
            if(nextStep) TutorialNextStep();
        }

        private void SetState(ITutorialStep subStep)
        {
            InjectionSystem.Inject(subStep);
            stateMachine.SetState(subStep);
        }
    }
}