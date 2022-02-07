using Core;
using Core.Controllers;
using Extensions;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class AttackButtonPulseController : IAttackButtonPulseController, IController
    {
        [Inject] public AttackButtonViewModel AttackButtonViewModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        private CursorIdentifier targetCursorIdentifier = null;

        void IController.Enable() 
        {
            AttackButtonViewModel.SetPulseAnimation(true);
            PlayerEventHandler.RaycastData.OnChange += OnChangeRaycastDataHandler;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            AttackButtonViewModel.SetPulseAnimation(false);
            PlayerEventHandler.RaycastData.OnChange -= OnChangeRaycastDataHandler;
        }

        private void OnChangeRaycastDataHandler()
        {
            targetCursorIdentifier = PlayerEventHandler.RaycastData.Value?.GameObject.CheckNull()?.GetComponent<CursorIdentifier>();
            AttackButtonViewModel.SetPulseAnimation(targetCursorIdentifier != null && targetCursorIdentifier.CursorID == CursorID.DestroyableCursor);
        }
    }
}
