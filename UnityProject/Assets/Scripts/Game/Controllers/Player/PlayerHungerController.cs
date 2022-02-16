using System;
using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerHungerController : IPlayerHungerController, IController
    {
        [Inject] public PlayerHungerModel PlayerHungerModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerFoodModel PlayerFoodModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }

        protected bool IsHungerProcess { get; private set; }

        private bool IsCanHungerProcess => PlayerFoodModel.FoodCurrent > 0
            && !PlayerDeathModel.IsImmunable
            && !PlayerHealthModel.IsDead
            && !TutorialModel.IsTutorialNow;

        void IController.Enable()
        {
            PlayerFoodModel.OnChangeFood += OnChangeFoodHandler;
            PlayerDeathModel.OnEndImunitet += OnEndImunitetHandler;
            PlayerHealthModel.OnDeath += OnDeathHandler;

            if (IsCanHungerProcess)
            {
                BeginHungerProcess();
            }
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerFoodModel.OnChangeFood -= OnChangeFoodHandler;
            PlayerDeathModel.OnEndImunitet -= OnEndImunitetHandler;
            PlayerHealthModel.OnDeath -= OnDeathHandler;

            EndHungerProcess();
        }

        private void OnDeathHandler() => EndHungerProcess();
        private void OnEndImunitetHandler() => BeginHungerProcess();

        private void OnChangeFoodHandler()
        {
            UpdateHungerProcess();

            {//??[REFACTOR
                if (PlayerFoodModel.FoodCurrent > 0)
                {
                    PlayerFoodModel.StopHunger();
                }
                if (PlayerFoodModel.FoodCurrent <= PlayerHungerModel.FoodToHunger)
                {
                    PlayerFoodModel.StartHunger();
                }
            }
        }

        private void UpdateHungerProcess()
        {
            if (IsCanHungerProcess)
            {
                BeginHungerProcess();
            }
            else
            {
                EndHungerProcess();
            }
        }

        private void BeginHungerProcess()
        {
            if (!IsHungerProcess)
            {
                IsHungerProcess = true;
                GameUpdateModel.OnUpdate += HungerProcess;
            }
        }

        private void EndHungerProcess()
        {
            if (IsHungerProcess)
            {
                IsHungerProcess = false;
                GameUpdateModel.OnUpdate -= HungerProcess;
            }
        }

        private void HungerProcess()
        { 
            float adjustFood = PlayerHungerModel.HungerPerSecond * Time.deltaTime;

            if(!TutorialModel.IsComplete  && PlayerFoodModel.Food - adjustFood < PlayerHungerModel.FoodToHunger)
                return;

            PlayerFoodModel.AdjustFood(-adjustFood);
        }
    }
}
