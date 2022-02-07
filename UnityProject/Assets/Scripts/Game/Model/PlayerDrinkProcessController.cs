using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerDrinkProcessController : IPlayerDrinkProcessController, IController
    {
        [Inject] public PlayerDrinkProcessModel PlayerDrinkProcessModel { get; private set; }
        [Inject] public PlayerWaterModel PlayerWaterModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        private bool IsDrinkProceesing { get; set; }

        void IController.Enable()
        {
            if (PlayerDrinkProcessModel.IsInitializeData)
            {
                StartDrinkProcessData();
            }
            else
            {
                PlayerDrinkProcessModel.OnLoadData += OnLoadDataHandler;
            }

            PlayerDrinkProcessModel.OnAddDrink += OnAddDrinkHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            StopDrinkProcess();
            PlayerDrinkProcessModel.OnLoadData -= OnLoadDataHandler;
            PlayerDrinkProcessModel.OnAddDrink -= OnAddDrinkHandler;
            GameUpdateModel.OnUpdate -= DrinkProcessing;
        }

        private void OnLoadDataHandler() => StartDrinkProcessData();

        private void StartDrinkProcessData()
        {
            if (PlayerDrinkProcessModel.IsHasDrink)
            {
                StartDrinkProcess();
            }
        }

        private void OnAddDrinkHandler() => StartDrinkProcess();

        private void StartDrinkProcess()
        {
            if (!IsDrinkProceesing)
            {
                GameUpdateModel.OnUpdate += DrinkProcessing;
                IsDrinkProceesing = true;
            }
        }

        private void StopDrinkProcess()
        {
            PlayerDrinkProcessModel.EndDrink();
            GameUpdateModel.OnUpdate -= DrinkProcessing;
            IsDrinkProceesing = false;
        }

        private void DrinkProcessing()
        {
            ApplyDrink();
            PlayerDrinkProcessModel.UpdateDrinkValues(Time.deltaTime); 

            if (!PlayerDrinkProcessModel.IsHasDrink)
            {
                StopDrinkProcess();
            }
        }

        private void ApplyDrink()
        {
            foreach (var h in PlayerDrinkProcessModel.DrinkDatas)
            {
                var drink = PlayerDrinkProcessModel.GetDrinkValue(h, Time.deltaTime);
                var isOverflow = PlayerDrinkProcessModel.GetIsOverflow(h);
                PlayerWaterModel.AdjustWater(drink, isOverflow);
            }
        }
    }
}
