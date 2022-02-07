using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Controllers.TutorialSteps;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class VikingTutorialController : IVikingTutorialController, IController
    {
        [Inject] public InjectionSystem InjectionSystem { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ControllersModel ControllersModel { get; private set; }
        [Inject] public VikingTutorialModel VikingTutorialModel { get; private set; }

        #region Events
        private TutorialEvent learnInputStarted = new TutorialEvent("learnInputStarted");
        private TutorialEvent getPickaxeStarted = new TutorialEvent("getPickaxeStarted");
        private TutorialEvent approchedBarrel = new TutorialEvent("approchedBarrel");
        private TutorialEvent approachedPickaxe = new TutorialEvent("approachedPickaxe");
        private TutorialEvent tookPickaxe = new TutorialEvent("tookPickaxe");
        private TutorialEvent getRocksStarted = new TutorialEvent("getRocksStarted");
        private TutorialEvent mineWoodStarted = new TutorialEvent("mineWoodStarted");
        private TutorialEvent buildTownhallStared = new TutorialEvent("buildTownhallStared");
        private TutorialEvent lootBoxStarted = new TutorialEvent("lootBoxStarted");
        private TutorialEvent equipmentStarted = new TutorialEvent("equipmentStarted");
        private TutorialEvent huntRabbitStarted = new TutorialEvent("huntRabbitStarted");
        private TutorialEvent cookMeatStarted = new TutorialEvent("cookMeatStarted");
        private TutorialEvent killFirstEnemyStarted = new TutorialEvent("killFirstEnemyStarted");
        private TutorialEvent puzzleStarted = new TutorialEvent("puzzleStarted");
        private TutorialEvent buildToolsStarted = new TutorialEvent("buildToolsStarted");
        private TutorialEvent finalMessageStarted = new TutorialEvent("finalMessageStarted");
            
        #endregion

        private TutorialStepsStateMachine stepsStateMachine = new TutorialStepsStateMachine();

        private List<ITutorialStep> steps = new List<ITutorialStep>();
        private Dictionary<TutorialEvent,int> eventIndexByTutorialEvent = new Dictionary<TutorialEvent,int>();
        
        void IController.Enable() 
        {
            Init();

            if(TutorialModel.IsComplete) return;

            if(!TutorialModel.IsActive && !TutorialModel.IsComplete) 
                TutorialModel.ActivateTutorial();
            
            InitTutorialSteps();

            TutorialModel.OnStepChanged += ApplyTutorialModificator;
            TutorialModel.OnStepChanged += UpdaetTutorialStepStateMachine;
            TutorialModel.OnIsActiveChanged += UpdaetTutorialStepStateMachine;
            // TutorialModel.OnComplete += TutorialCompleateHandler;
            
            ApplyTutorialModificator();
            UpdaetTutorialStepStateMachine();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            TutorialModel.OnStepChanged -= ApplyTutorialModificator;
            TutorialModel.OnStepChanged -= UpdaetTutorialStepStateMachine;
            TutorialModel.OnIsActiveChanged -= UpdaetTutorialStepStateMachine;
        }

        private void Init()
        {

        }

        private void InitTutorialSteps()
        {

            eventIndexByTutorialEvent = new Dictionary<TutorialEvent, int>()
                {
                    {learnInputStarted,1},
                    {getPickaxeStarted,2},
                    {approchedBarrel,3},
                    {approachedPickaxe,4},
                    {tookPickaxe,5},
                    {getRocksStarted,6},
                    {mineWoodStarted, 7},
                    {buildTownhallStared, 8},
                    {lootBoxStarted, 9},
                    {equipmentStarted, 10},
                    {huntRabbitStarted,11},
                    {cookMeatStarted, 12},
                    {killFirstEnemyStarted, 13},
                    {puzzleStarted, 14},
                    {buildToolsStarted, 15},
                    {finalMessageStarted, 16},
                };

            ValidateEvents();
            SubscribeForEvents();
            
            steps = new List<ITutorialStep>()
            {
                //TODO: step only action
                //TODO: fix not enough items

                new TutorialStep_LearnInput(learnInputStarted),
                new TutorialStep_GetPickaxe(getPickaxeStarted,approchedBarrel,approachedPickaxe,tookPickaxe), //TODO: step only action
                new TutorialStep_MineStone(getRocksStarted, "Stone",5,12,"toeken_stone",OutLineMinableObjectID.Stone,null,"Mine stone"),
                new TutorialStep_MineWood(mineWoodStarted, "Wood",5,11,"toeken_tree",OutLineMinableObjectID.Tree,null, "Mine wood"),
                new TutorialStep_BuildTownhall(buildTownhallStared), //TODO: step only action
                new TutorialStep_LootBox(lootBoxStarted),
                new TutorialStep_Equipment(equipmentStarted),
                new TutorialStep_HuntRabbit(huntRabbitStarted), //TODO: step only action
                new TutorialStep_CookMeat(cookMeatStarted),
                new TutorialStep_KillFirstEnemy(killFirstEnemyStarted),
                new TutorialStep_Puzzle(puzzleStarted), //TODO: step only action
                new TutorialStep_BuildTools(buildToolsStarted),
                new TutorialStep_FinalMessage(finalMessageStarted),
            };

            foreach (var step in steps)
            {
                InjectionSystem.Inject(step);
            }
        }

        private void ApplyTutorialModificator()
        {
            // ControllersModel.ApplyModificator(TutorialModel.Modificator);
        }

        private void UpdaetTutorialStepStateMachine()
        {
            if(TutorialModel.IsComplete)
            {
                stepsStateMachine.SetState(null);
            }
            else
            {
                ITutorialStep step = GetTutorialStep(TutorialModel.Step);
                stepsStateMachine.SetState(step);
            }
        }

        private ITutorialStep GetTutorialStep(int index) => steps[index];

        private void ValidateEvents()
        {
            List<int> indexes = new List<int>();
            foreach (var keyValuePair in eventIndexByTutorialEvent)
            {
                int actionIndex = keyValuePair.Value;

                if(indexes.Contains(actionIndex)) Debug.LogError("Error: index dublicate");
                indexes.Add(actionIndex);
            }
        }

        private void SubscribeForEvents()
        {
            foreach (var keyValuePair in eventIndexByTutorialEvent)
            {
                var tutorialEvent = keyValuePair.Key;
                var tutorialEventIndex = keyValuePair.Value;
                tutorialEvent.OnFired += () => { 
                    Debug.Log($"<color=orange> Event #{tutorialEventIndex} {tutorialEvent.name} </color>");
                    VikingTutorialModel.SetAnalyticsStep(tutorialEventIndex);
                };
            }
        }
    }
}
