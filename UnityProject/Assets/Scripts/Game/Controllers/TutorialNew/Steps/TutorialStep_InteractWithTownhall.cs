using Core;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using Game.Views;
using Game.VillageBuilding;
using UnityEngine;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_InteractWithTownhall : TutorialStepBase
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public QuestNotificationsModel QuestNotificationsModel { get; private set; }

        private SimpleTutorialController tutorialController;
        private SimpleTutorialController SimpleTutorialController
        {
            get
            {
                if (tutorialController == null)
                    tutorialController = GameObject.FindObjectOfType<SimpleTutorialController>();

                return tutorialController;
            }
        }

        private HouseBuilding Townhall { get => SimpleTutorialController.Townhall; }

        public TutorialStep_InteractWithTownhall(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }

        public override void OnStart()
        {
            QuestNotificationsModel.Show("Builid Towhall", SimpleTutorialController.townhallIcon);
            Townhall.GetComponent<TokenTarget>().enabled = true;
            ViewsSystem.OnBeginShow.AddListener(ViewConfigID.HouseBuildingConfig, OnOpenHouseBuildingViewStep);
        }
        public override void OnEnd()
        {
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.HouseBuildingConfig, OnOpenHouseBuildingViewStep);
            Townhall.GetComponent<TokenTarget>().enabled = false;
        }

        private void OnOpenHouseBuildingViewStep()
        {
            CheckConditions();
        }

        private void CheckConditions()
        {
            bool nextStep = true;

            if(nextStep) TutorialNextStep();
        }
    }
}