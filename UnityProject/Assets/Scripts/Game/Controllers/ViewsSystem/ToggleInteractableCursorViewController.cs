using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Models.RemoteSettings;
using Game.Purchases;
using Game.Views;
using System;
using System.Linq;
using UltimateSurvival;

namespace Game.Controllers
{
    public class ToggleInteractableCursorViewController : ViewControllerBase<ToggleInteractableCursorView>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public AudioSystem AudioSystem {get; private set;}

        private void Interact()
        {
            var interactable = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInChildren<IToggleInteractable>();
            if(interactable == null) return;

            interactable.Interact();

            var sound = interactable.On ? AudioID.TurnOnFire : AudioID.TurnOnFire;
            AudioSystem.PlayOnce(sound);
            View.SetFireCursor(interactable.On);
        }

        protected override void Show()
        {
            View.OnInteract += OnDownHandler;

            var interactable = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInChildren<IToggleInteractable>();
            if(interactable == null) return;

            View.SetFireCursor(interactable.On);
        }

        protected override void Hide()
        {
            View.OnInteract -= OnDownHandler;
        }

        private void OnDownHandler() => Interact();
    }
}
