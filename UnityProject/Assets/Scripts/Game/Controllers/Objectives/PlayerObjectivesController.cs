using Core;
using Core.Controllers;
using Game.Models;
using Game.Objectives;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerObjectivesController : IPlayerObjectivesController, IController
    {
        [Inject] public PlayerObjectivesModel PlayerObjectivesModel { get; private set; }
        [Inject] public ObjectivesViewModel ObjectivesViewModel { get; private set; }
        [Inject] public ObjectivesModel ObjectivesModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public RealTimeModel RealTimeModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public ObjectivesWindowModel ObjectivesWindowModel { get; private set; }

        private Dictionary<byte, ObjectiveModel> Pool { get; } = new Dictionary<byte, ObjectiveModel>();

        private bool UpdateRemaningTime
        {
            get
            {
                if(PoolIsEmpty)
                {
                    return true;
                }
                else
                {
                    return ObjectivesWindowModel.RequiredObjectivesCompleated;
                }
            }
        }
        private bool PoolIsEmpty {get => PlayerObjectivesModel.PoolIsEmpty;}
        private bool HasTasks {get => PlayerObjectivesModel.HasTasks;}
        

        void IController.Enable()
        {
            // PlayerObjectivesModel.OnEndTime += OnEndTimeHandler;
            // PlayerObjectivesModel.OnPreEndTask += OnEndTaskHandler;
            // PlayerObjectivesModel.OnRerollObjective += OnRerollObjective; 
            // ObjectivesModel.OnRemoveObjective += OnRemoveObjective;
            // PlayerObjectivesModel.OnAddToPool_Temp += OnAddToPool_TempHandler;
            // PlayerObjectivesModel.OnUpdatePool += UpdatePoolHandler;
        }

        void IController.Start()
        {
            // if (!ObjectivesViewModel.IsHasAny)
            // {
            //     RealTimeModel.OnTimeReady += OnRealtimeReady;
            //     RealTimeModel.OnTimeError += OnRealtimeError;
            //     RealTimeModel.DropTime();
            // }
            // else
            // {
            //     ObjectivesViewModel.OnPickUpTutorialStep1 += OnChangeHasTutorialHandler;
            //     ObjectivesViewModel.OnPickUpTutorialStep2 += OnChangeHasTutorialHandler;
            //     ObjectivesViewModel.OnPickUpTutorialStep3 += OnChangeHasTutorialHandler;
            //     ObjectivesViewModel.OnPickUpTutorialStep4 += OnChangeHasTutorialHandler;
            //     ObjectivesViewModel.OnPickUpTutorialStep5 += OnChangeHasTutorialHandler;
            //     ObjectivesViewModel.OnPickUpTutorialStep6 += OnChangeHasTutorialHandler;
            // }

            // // set Pool (original version)
            // foreach (var item in PlayerObjectivesModel.Pool)
            // {
            //     if (item.IsHasValue)
            //     {
            //         bool error = !ObjectivesModel.TryGet(item.Value, out var model);
            //         Pool[item.Id] = model;
            //     }
            // }

            // PlayerObjectivesModel.ResetPlayerObjectivesOnStart();
        }

        void IController.Disable()
        {
            // PlayerObjectivesModel.OnEndTime -= OnEndTimeHandler;
            // PlayerObjectivesModel.OnPreEndTask -= OnEndTaskHandler;
            // PlayerObjectivesModel.OnRerollObjective -= OnRerollObjective; 
            // Pool.Clear();
            // GameUpdateModel.OnUpdate -= UpdateTime;
            // ObjectivesViewModel.OnPickUpTutorialStep1 -= OnChangeHasTutorialHandler;
            // ObjectivesViewModel.OnPickUpTutorialStep2 -= OnChangeHasTutorialHandler;
            // ObjectivesViewModel.OnPickUpTutorialStep3 -= OnChangeHasTutorialHandler;
            // ObjectivesViewModel.OnPickUpTutorialStep4 -= OnChangeHasTutorialHandler;
            // ObjectivesViewModel.OnPickUpTutorialStep5 -= OnChangeHasTutorialHandler;
            // ObjectivesViewModel.OnPickUpTutorialStep6 -= OnChangeHasTutorialHandler;
            // RealTimeModel.OnTimeReady -= OnRealtimeReady;
            // RealTimeModel.OnTimeError -= OnRealtimeError;
            // ObjectivesModel.OnRemoveObjective -= OnRemoveObjective;
            // PlayerObjectivesModel.OnAddToPool_Temp -= OnAddToPool_TempHandler;
            // PlayerObjectivesModel.OnUpdatePool -= UpdatePoolHandler;
        }

        private void OnRealtimeReady()
        {
            if(UpdateRemaningTime)
            {
                long remainingTicks = PlayerObjectivesModel.NextTierTime - RealTimeModel.Now().Ticks;
                float remainingSeconds = GameTimeModel.GetSecondsTotal(remainingTicks);
             
                if(HasTasks && PlayerObjectivesModel.BonusTimeAvaliable)
                {
                    bool giveBonusTime = remainingSeconds < 0;
                    
                    if(giveBonusTime)
                    {
                        remainingSeconds = PlayerObjectivesModel.BonusTime;
                        PlayerObjectivesModel.SetBonusTimeAvaliable(false);
                    }
                }

                PlayerObjectivesModel.SetNextTierTime(RealTimeModel.Now().Ticks + GameTimeModel.GetTicks(remainingSeconds));
                PlayerObjectivesModel.SyncRemainingTime(remainingSeconds);
            }

            GameUpdateModel.OnUpdate -= UpdateTime;
            GameUpdateModel.OnUpdate += UpdateTime;
        }

        private void OnRealtimeError(string msg) => GameUpdateModel.OnUpdate -= UpdateTime;

        private void OnChangeHasTutorialHandler() 
        {
            if (!ObjectivesViewModel.IsHasAny)
            {
                ObjectivesViewModel.OnPickUpTutorialStep1 -= OnChangeHasTutorialHandler;
                ObjectivesViewModel.OnPickUpTutorialStep2 -= OnChangeHasTutorialHandler;
                ObjectivesViewModel.OnPickUpTutorialStep3 -= OnChangeHasTutorialHandler;
                ObjectivesViewModel.OnPickUpTutorialStep4 -= OnChangeHasTutorialHandler;
                ObjectivesViewModel.OnPickUpTutorialStep5 -= OnChangeHasTutorialHandler;
                ObjectivesViewModel.OnPickUpTutorialStep6 -= OnChangeHasTutorialHandler;

                SetObjectivesNextTime();

                RealTimeModel.OnTimeReady += OnRealtimeReady;
                RealTimeModel.OnTimeError += OnRealtimeError;
                RealTimeModel.DropTime();
            }
        }

        private void OnEndTimeHandler()
        {
            SetObjectivesNextTime();
        }

        private void OnAddToPool_TempHandler(byte id, ObjectiveModel model)
        {
            Pool[id] = model;
        }

        private void OnRerollObjective(ObjectiveModel objectiveModel, int index)
        {
            ObjectivesViewModel.BlockPlayAnimation = true;
            PlayerObjectivesModel.ResetPlayerObjectives();
            UpdatePoolHandler();
            ObjectivesWindowModel.RerollObjectiveInWindow(objectiveModel, index);
            ObjectivesViewModel.BlockPlayAnimation = false;

            PlayerObjectivesModel.ResetPlayerObjectives();
        }

        private void SetObjectivesNextTime()
        {
            float delay = PlayerObjectivesModel.IsFirst ? PlayerObjectivesModel.FirstTimeDelay : PlayerObjectivesModel.TimeReset;
            PlayerObjectivesModel.SetNextTierTime(RealTimeModel.Now().Ticks + GameTimeModel.GetTicks(delay));
            PlayerObjectivesModel.SyncRemainingTime(delay);
            PlayerObjectivesModel.SetBonusTimeAvaliable(true);
        }

        private void UpdateTime()
        {
            if(!UpdateRemaningTime) // TODO: use event instead of setting value each update
            {
                SetObjectivesNextTime();
            }
            else 
            {
                PlayerObjectivesModel.ProcessingTime(Time.unscaledDeltaTime);
            }
        }

        private void UpdatePoolHandler()
        {
            foreach (var item in Pool)
            {
                PlayerObjectivesModel.SetToPool(item.Key, ObjectivesModel.GetId(item.Value));
            }
        }

        private void OnRemoveObjective(ObjectiveModel removedObjective)
        {
            PlayerObjectivesModel.UpdatePool();
        }

        private void OnEndTaskHandler(byte id)
        {
            bool error = !Pool.TryGetValue(id, out var objectiveModel);
            if(error || objectiveModel == null) return;

            ObjectivesModel.RemoveObjective(Pool[id]);
            Pool.Remove(id);
        }
    }
}
