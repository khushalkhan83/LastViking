using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Objectives;
using Game.Views;
using System.Collections;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class FindTombController : IFindTombController, IController
    {
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public PlayerEventObjectivesModel PlayerEventObjectivesModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public FindTombModel FindTombModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private ObjectiveID UsedObjectiveID => ObjectiveID.TombInteract;

        private int _waitFrameCoroutineId = -1;

        void IController.Enable()
        {
            StorageModel.TryProcessing(FindTombModel._Data);
        }

        void IController.Start()
        {
            if (FindTombModel.HasEnded) return;

            if (FindTombModel.IsEventInProgress) 
            {
                PlayerEventObjectivesModel.OnEndObjective += OnEndObjective;
            }
            else
            {
                if (TutorialModel.IsComplete)
                {
                    PlayerDeathModel.OnRevival += OnRevivalHandler;
                }
                else
                {
                    TutorialModel.OnComplete += OnCompleteTutorial;
                }
            }
        }
        void IController.Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            TutorialModel.OnComplete -= OnCompleteTutorial;
            CoroutineModel.BreakeCoroutine(_waitFrameCoroutineId);
        }

        private void OnCompleteTutorial()
        {
            TutorialModel.OnComplete -= OnCompleteTutorial;
            PlayerDeathModel.OnRevival += OnRevivalHandler;
        }


        private void OnRevivalHandler()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            _waitFrameCoroutineId = CoroutineModel.InitCoroutine(WaitFrameForInitObjective());
        }

        private IEnumerator WaitFrameForInitObjective()
        {
            yield return null;
            yield return null;
            yield return null;
            InitObjective();
        }

        private void InitObjective()
        {
            var objId = UsedObjectiveID;
            FindTombModel.StartEvent();
            PlayerEventObjectivesModel.StartObjective(objId);
            PlayerEventObjectivesModel.OnEndObjective += OnEndObjective;
        }

        private void OnEndObjective()
        {
            if (PlayerEventObjectivesModel.CurrentObjectiveID == UsedObjectiveID)
            {
                PlayerEventObjectivesModel.OnEndObjective -= OnEndObjective;
                FindTombModel.EndEvent();
            }
        }
    }
}
