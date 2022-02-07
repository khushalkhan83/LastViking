using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using System;
using System.Collections;
using UnityEngine;
using Game.Progressables;

namespace Game.Controllers
{
    public class DungeonProgressController : IDungeonProgressController, IController
    {
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }
        [Inject] public DungeonsProgressModel DungeonsProgressModel { get; private set; }

        void IController.Enable()
        {
            foreach(var dungeonProgressModel in DungeonsProgressModel.DungeonProgressModels)
            {
                dungeonProgressModel.OnPassedLocation += OnPassedLocation;
                dungeonProgressModel.CalcRemainingTime += CalcRemainingTime;
                StorageModel.TryProcessing(dungeonProgressModel._Data);
            }
        }

        void IController.Start()
        {
            foreach(var dungeonProgressModel in DungeonsProgressModel.DungeonProgressModels)
            {
                if (dungeonProgressModel.InProgress)
                {
                    dungeonProgressModel.ProgressStatus = ProgressStatus.InProgress;
                }
                else
                {
                    if (GameTimeModel.RealTimeNowTick >= dungeonProgressModel.NextProgressResetTime)
                    {
                        dungeonProgressModel.ResetProgress();
                    }
                    else
                    {
                        dungeonProgressModel.ProgressStatus = ProgressStatus.WaitForResetProgress;
                        ResetProcess(dungeonProgressModel);
                    }
                }
            }
        }

        void IController.Disable()
        {
            foreach(var dungeonProgressModel in DungeonsProgressModel.DungeonProgressModels)
            {
                dungeonProgressModel.OnPassedLocation -= OnPassedLocation;
                dungeonProgressModel.CalcRemainingTime -= CalcRemainingTime;
            }
        }

        private void OnPassedLocation(DungeonProgressModel dungeonProgressModel)
        {
            long respawnTicks = GameTimeModel.GetTicks(dungeonProgressModel.ProgressResetTime);
            dungeonProgressModel.NextProgressResetTime = GameTimeModel.TicksRealNow + respawnTicks;
            ResetProcess(dungeonProgressModel);
        }

        private void ResetProcess(DungeonProgressModel dungeonProgressModel)
        {
            float remainingSeconds = GameTimeModel.GetSecondsTotal(dungeonProgressModel.NextProgressResetTime - GameTimeModel.RealTimeNowTick);
            dungeonProgressModel.StartCoroutine(WaitForResetProgress(remainingSeconds, dungeonProgressModel));
        }

        public IEnumerator WaitForResetProgress(float remainingSeconds, DungeonProgressModel dungeonProgressModel)
        {
            yield return new WaitForSecondsRealtime(remainingSeconds);
            dungeonProgressModel.ResetProgress();
        }

        private float CalcRemainingTime(DungeonProgressModel dungeonProgressModel) => GameTimeModel.GetSecondsTotal(dungeonProgressModel.NextProgressResetTime - GameTimeModel.RealTimeNowTick);
    }
}
