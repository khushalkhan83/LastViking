using System.Collections;
using BehaviorDesigner.Runtime;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class EnvironmentTimeController : IEnvironmentTimeController, IController
    {
        [Inject] public EnvironmentTimeModel EnvironmentTimeModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public GameTimeModel GameTimeModel { get; private set; }
        [Inject] public PlayerStatusViewModel PlayerStatusViewModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private GlobalVariables GlobalVariables => GlobalVariables.Instance;

        private const string DayTimeVariableName = "DayTime";

        private int coroutineId = -1;

        void IController.Enable() 
        {
            PlayerScenesModel.OnPreEnvironmentChange += OnPreEnvironmentLoad;
            PlayerScenesModel.OnEnvironmentChange += OnEnvironmentLoad;
            EnvironmentTimeModel.OnDayTimeChanged += OnDayTimeChanged;

            coroutineId = CoroutineModel.InitCoroutine(CheckDayNight());
        }

        private void OnPreEnvironmentLoad()
        {
            if (PlayerScenesModel.ActiveEnvironmentSceneID == EnvironmentSceneID.VikingsIslandScene)
            {
                PlayerStatusViewModel.DisableEnviroTime();
                GameTimeModel.PauseEnviroTime();
            }
        }

        private void OnEnvironmentLoad()
        {
            if (PlayerScenesModel.ActiveEnvironmentSceneID == EnvironmentSceneID.VikingsIslandScene)
            {
                PlayerStatusViewModel.EnableEnviroTime();
                GameTimeModel.UnpauseEnviroTime();
            }
        }

        void IController.Start() 
        {
            if (PlayerScenesModel.ActiveEnvironmentSceneID != EnvironmentSceneID.VikingsIslandScene)
            {
                PlayerStatusViewModel.DisableEnviroTime();
                GameTimeModel.PauseEnviroTime();
            }
        }

        void IController.Disable() 
        {
            PlayerScenesModel.OnPreEnvironmentChange -= OnPreEnvironmentLoad;
            PlayerScenesModel.OnEnvironmentChange -= OnEnvironmentLoad;
            EnvironmentTimeModel.OnDayTimeChanged -= OnDayTimeChanged;

            CoroutineModel.BreakeCoroutine(coroutineId);
        }

        private void OnDayTimeChanged()
        {
            GlobalVariables.SetVariableValue(DayTimeVariableName,EnvironmentTimeModel.IsDayTime);
        }

        private IEnumerator CheckDayNight()
        {
            do
            {
                Check();
                yield return new WaitForSeconds(2);
                
            } while (true);
        }

        private void Check()
        {
            var hours = GameTimeModel.EnviroHours;
            bool day = hours > 7 && hours < 20;
            EnvironmentTimeModel.SetDay(day);
        }
    }
}
