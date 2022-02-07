using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerEatProcessController : IPlayerEatProcessController, IController
    {
        [Inject] public PlayerEatProcessModel PlayerEatProcessModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        private bool IsEatProceesing { get; set; }

        void IController.Enable()
        {
            if (PlayerEatProcessModel.IsInitializeData)
            {
                StartEatProcessData();
            }
            else
            {
                PlayerEatProcessModel.OnLoadData += OnLoadDataHandler;
            }

            PlayerEatProcessModel.OnAddEat += OnAddEatHandler;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            StopEatProcess();
            PlayerEatProcessModel.OnLoadData -= OnLoadDataHandler;
            PlayerEatProcessModel.OnAddEat -= OnAddEatHandler;
            GameUpdateModel.OnUpdate -= EatProcessing;
        }

        private void OnLoadDataHandler() => StartEatProcessData();

        private void StartEatProcessData()
        {
            if (PlayerEatProcessModel.IsHasEat)
            {
                StartEatProcess();
            }
        }

        private void OnAddEatHandler() => StartEatProcess();

        private void StartEatProcess()
        {
            if (!IsEatProceesing)
            {
                GameUpdateModel.OnUpdate += EatProcessing;
                IsEatProceesing = true;
            }
        }

        private void StopEatProcess()
        {
            PlayerEatProcessModel.EndEat();
            GameUpdateModel.OnUpdate -= EatProcessing;
            IsEatProceesing = false;
        }

        private void EatProcessing()
        {
            ApplyEat();
            PlayerEatProcessModel.UpdateEatValues(Time.deltaTime); 

            if (!PlayerEatProcessModel.IsHasEat)
            {
                StopEatProcess();
            }
        }

        private void ApplyEat()
        {
            foreach (var h in PlayerEatProcessModel.EatDatas)
            {
                var eat = PlayerEatProcessModel.GetEatValue(h, Time.deltaTime);
                var isOverflow = PlayerEatProcessModel.GetIsOverflow(h);
                PlayerFoodModel.AdjustFood(eat, isOverflow);
            }
        }
    }
}
