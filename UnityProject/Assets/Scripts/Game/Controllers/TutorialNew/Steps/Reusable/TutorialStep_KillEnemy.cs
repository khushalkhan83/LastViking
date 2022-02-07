using System;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_KillEnemy : TutorialStepBase
    {
        // [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        private EnemyHealthModel healthModel;
        private string message;
        private GameObject enemy;
        private TokenTarget token;

        public TutorialStep_KillEnemy(TutorialEvent StepStartedEvent, Action onCompleatedAsSubStep, GameObject enemy, string message) : base(StepStartedEvent,onCompleatedAsSubStep)
        {
            Init(enemy,message);
        }

        public TutorialStep_KillEnemy(TutorialEvent StepStartedEvent, GameObject enemy, string message) : base (StepStartedEvent)
        {
            Init(enemy,message);
        }

        private void Init(GameObject enemy, string message)
        {
            this.enemy = enemy;
            healthModel = enemy.GetComponentInChildren<EnemyHealthModel>();
            this.message = message;
            token = enemy.GetComponentInChildren<TokenTarget>();
        }

        public override void OnStart()
        {
            healthModel.OnChangeHealth += CheckConditions;

            ShowTaskMessage(true,message);
            enemy.SetActive(true);
            token.enabled = true;
        }

        public override void OnEnd()
        {
            healthModel.OnChangeHealth -= CheckConditions;

            ShowTaskMessage(false);
            // enemy.SetActive(false); // TODO: handle this problem if we will reinit step ?
            token.enabled = false;
        }

        private void CheckConditions()
        {
            bool nextStep = healthModel.IsDead;

            if(nextStep) TutorialNextStep();
        }
    }
}