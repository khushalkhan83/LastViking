using Core;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_KillFirstEnemy : TutorialStepBase
    {
        private TutorialStep_KillEnemy step;

        public TutorialStep_KillFirstEnemy(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

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
            var sceneContext = GameObject.FindObjectOfType<SimpleTutorialController>();
            var enemy = sceneContext.woolf;
        
            enemy.SetActive(true); // TODO: move inside kill enemy tutorial step ?
            step = new TutorialStep_KillEnemy(null,CheckConditions,enemy,LocalizationModel.GetString(Models.LocalizationKeyID.Tutorial_Kill_Wolf));
            InjectionSystem.Inject(step);
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }
    }
}