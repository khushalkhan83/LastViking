using System.Collections;
using System.Collections.Generic;
using Core;
using Game.Models;
using Game.Puzzles;
using UnityEngine;
using System.Linq;

namespace Game.Controllers.TutorialSteps
{
    public class TutorialStep_Puzzle : TutorialStepBase
    {
        [Inject] public PuzzlesModel PuzzlesModel { get; private set; }
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private string tutorialPuzzleName = "Skull Buttons Puzzle";
        private string triggerTokenName = "Trigger";
        private string chestTokenName = "LootContainer";
        private int tokenId = 0;
        private TriggeredDoorWithChest tutorialPuzzle;
        private WorldObjectModel chestWorldObjectModel;
        private int coroutineId = -1;

        public TutorialStep_Puzzle(TutorialEvent StepStartedEvent) : base(StepStartedEvent) { }
        
        public override void OnStart()
        {
            CoroutineModel.BreakeCoroutine(coroutineId);
            coroutineId = CoroutineModel.InitCoroutine(InitWithDelay());
        }

        public override void OnEnd()
        {
            tutorialPuzzle.OnActivated -= ProcessState;
            WorldObjectsModel.OnRemove.RemoveListener(WorldObjectID.drop_container, OnRemoveChest);
            foreach(var triggerSettings in tutorialPuzzle.TriggerSettings)
            {
                triggerSettings.trigger.OnIsActiveChanged -= ProcessState;
            }
            CoroutineModel.BreakeCoroutine(coroutineId);
        }

        private IEnumerator InitWithDelay()
        {
            yield return null;
            tutorialPuzzle = PuzzlesModel.ActivePuzzles.FirstOrDefault(m => m.PuzzleName.Equals(tutorialPuzzleName));
            tutorialPuzzle.OnActivated += ProcessState;
            WorldObjectsModel.OnRemove.AddListener(WorldObjectID.drop_container, OnRemoveChest);
            foreach(var triggerSettings in tutorialPuzzle.TriggerSettings)
            {
                triggerSettings.trigger.OnIsActiveChanged += ProcessState;
            }
            ProcessState();
        }

        private void ProcessState()
        {
            bool tutorialPuzzleActivated = tutorialPuzzle.Activated;
            if(!tutorialPuzzleActivated)
            {
                HitButtonsState();
                return;
            }

            if(tutorialPuzzleActivated && WorldObjectsModel.SaveableObjectModels.TryGetValue(WorldObjectID.drop_container, out var chests))
            {
                chestWorldObjectModel = chests.FirstOrDefault(m => m.ID == tutorialPuzzle.ChestWorldObjectId);
                if(chestWorldObjectModel != null)
                {
                    CollectLootBoxState();
                    return;
                }
            }

            bool chestCollected = chestWorldObjectModel == null;
            if(tutorialPuzzleActivated && chestCollected)
            {
                FinishState();
            }

        }

        private void HitButtonsState()
        {
            ShowTaskMessage(true, LocalizationModel.GetString(LocalizationKeyID.Tutorial_Secrets));
            UpdateTriggersTokens();
        }

        private void CollectLootBoxState()
        {
            ShowTaskMessage(true, LocalizationModel.GetString(LocalizationKeyID.Tutorial_Collect_Lootbox));
            HideTriggersTokens();
            ShowChestToken();
        }

        private void FinishState()
        {
            HideTriggersTokens();
            HideChestTokne();
            CheckConditions();
        }

        private void UpdateTriggersTokens()
        {
            for(int i = 0; i < tutorialPuzzle.TriggerSettings.Length; i++)
            {
                TriggerBase trigger = tutorialPuzzle.TriggerSettings[i].trigger;
                if(!trigger.IsActive)
                {
                    TokensModel.ShowToken(triggerTokenName + i, tokenId, trigger.transform.position + Vector3.up * 0.5f);
                }
                else
                {
                    TokensModel.HideToken(triggerTokenName + i);
                }
            }
        }

        private void HideTriggersTokens()
        {
            for(int i = 0; i < tutorialPuzzle.TriggerSettings.Length; i++)
            {
                TokensModel.HideToken(triggerTokenName + i);
            }
        }

        private void ShowChestToken()
        {
            if(chestWorldObjectModel != null)
            {
                TokensModel.ShowToken(chestTokenName, tokenId, chestWorldObjectModel.transform.position + Vector3.up * 0.5f);
            }
        }

        private void HideChestTokne()
        {
            TokensModel.HideToken(chestTokenName);
        }

        private void OnRemoveChest(WorldObjectModel worldObjectModel)
        {
            if(worldObjectModel.ID == tutorialPuzzle.ChestWorldObjectId)
            {
                ProcessState();
            }
        }


        private void CheckConditions()
        {
            bool nextStep = tutorialPuzzle.Activated && chestWorldObjectModel == null;

            if(nextStep) TutorialNextStep();
        }
    }
}
