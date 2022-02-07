using Core;
using Core.Controllers;
using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class PlayerHealthRegenerationController : IPlayerHealthRegenerationController, IController
    {
        [Inject] public PlayerHealthRegenerationModel PlayerHealthRegenerationModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }

        private float lastRegenTime = 0f;

        void IController.Enable() 
        {
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void OnUpdate()
        {
            RegenUpdate();
        }

        private void RegenUpdate()
        {
            if(PlayerHealthModel.IsDead && PlayerHealthRegenerationModel.RegenValue == 0)
                return;

            if(Time.time - lastRegenTime >= PlayerHealthRegenerationModel.RegenInterval)
            {
                lastRegenTime = Time.time;
                PlayerHealthModel.AdjustHealth(PlayerHealthRegenerationModel.RegenValue);
            }
        }

    }
}
