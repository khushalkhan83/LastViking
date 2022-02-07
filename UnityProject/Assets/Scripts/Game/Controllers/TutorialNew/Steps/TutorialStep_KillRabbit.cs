using Core;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_HuntRabbit : TutorialStepBase
    {
        private const string targetItemID = "food_meat_chicken_raw";
        private const int tokenConfigID = 0;
        private const string killRabbitMessage = "You need food. Kill rabbit!";
        private const string takeHisMeatMessage = "Take your loot";

        private GameObject rabbit;

        protected TutorialStepsStateMachine stateMachine = new TutorialStepsStateMachine();

        public TutorialStep_HuntRabbit(TutorialEvent StepStartedEvent) : base(StepStartedEvent) {}
        private void Init()
        {
            var sceneContext = GameObject.FindObjectOfType<SimpleTutorialController>();
            rabbit = sceneContext.rabbit;
        }

        public override void OnStart()
        {
            Init();
            KillTargetTask();
        }
        
        public override void OnEnd()
        {
            ShowTaskMessage(false);
        }

        private void KillTargetTask()
        {
            ITutorialStep subStep = new TutorialStep_KillEnemy(null,onCompleatedAsSubStep: TakeItemTask, rabbit,killRabbitMessage);
            SetState(subStep);
        }

        private void TakeItemTask()
        {
            ITutorialStep subStep = new TutorialStep_TakeItem(null,OnCompleatedAsSubStep: CheckConditions,targetItemID,tokenConfigID);
            SetState(subStep);
            ShowTaskMessage(true,takeHisMeatMessage);
        }

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