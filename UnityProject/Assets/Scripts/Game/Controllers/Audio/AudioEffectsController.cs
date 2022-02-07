using System;
using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class AudioEffectsController : IAudioEffectsController, IController
    {
        [Inject] public AudioEffectsModel AudioEffectsModel { get; private set; }
        [Inject] public CinematicModel CinematicModel { get; private set; }
        void IController.Enable() 
        {
            AudioEffectsModel.OnRemoeAllEffects += OnRemoeAllEffects;
            CinematicModel.OnEndCinematic += OnRemoeAllEffects;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            AudioEffectsModel.OnRemoeAllEffects -= OnRemoeAllEffects;
            CinematicModel.OnEndCinematic -= OnRemoeAllEffects;
        }

        private void OnRemoeAllEffects()
        {
            AudioEffectsModel.SlowMotion(false);
        }
    }
}
