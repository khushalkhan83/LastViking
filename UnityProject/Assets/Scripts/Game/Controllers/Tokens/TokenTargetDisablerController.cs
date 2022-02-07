using Core;
using Core.Controllers;
using Extensions;
using Game.Models;
using Game.QuestSystem.Map.Extra;
using UltimateSurvival;

namespace Game.Controllers
{
    public class TokenTargetDisablerController : ITokenTargetDisablerController, IController
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }

        private TokenTargetDisabler tokenTargetDisabler;

        void IController.Enable() 
        {
            PlayerEventHandler.RaycastData.OnChange += OnChangeRaycastDataHandler;
            OnChangeRaycastDataHandler();
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            PlayerEventHandler.RaycastData.OnChange -= OnChangeRaycastDataHandler;
            if(tokenTargetDisabler != null) 
                tokenTargetDisabler.EnableTokenTarget();
        }

        private void OnChangeRaycastDataHandler()
        {
            var gameObject = PlayerEventHandler.RaycastData.Value?.GameObject;
            var newTokenTargetDisabler = gameObject.CheckNull()?.GetComponent<TokenTargetDisabler>();

            if(tokenTargetDisabler != null && tokenTargetDisabler != newTokenTargetDisabler)
            {
                tokenTargetDisabler.EnableTokenTarget();
            }

            tokenTargetDisabler = newTokenTargetDisabler;

            if(tokenTargetDisabler != null)
            {
                tokenTargetDisabler.DisableTokenTarget();
            }
        }

    }
}
