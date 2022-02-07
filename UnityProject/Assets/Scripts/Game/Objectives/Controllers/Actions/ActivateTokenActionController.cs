using Core;
using Game.Models;
using UnityEngine;

namespace Game.Objectives.Actions.Controllers
{
    /* Make inheritance ? */
    public class ActivateTokenActionController : BaseActionController<ActivateTokenActionData>
    {
        [Inject] public TokensModel TokensModel { get; private set; }
        [Inject] public WorldObjectsModel WorldObjectsModel { get; private set; }

        protected override void Action(ActivateTokenActionData actionData)
        {
            switch (actionData.TokenTarget)
            {
                case TokenTarget.Tombs:
                    ActionTombs(actionData);
                    break;
            }
        }

        private void ActionTombs(ActivateTokenActionData actionData)
        {
            var tombs = WorldObjectsModel.SaveableObjectModels[WorldObjectID.Tomb];
            int idx = 0;
            foreach (var tomb in tombs)
            {
                if (actionData.IsOn)
                {
                    TokensModel.ShowToken(actionData.TokenName + idx.ToString(), 0, tomb.Position);
                }
                else
                {
                    TokensModel.HideToken(actionData.TokenName + idx.ToString());
                }
                idx++;
            }
        }
    }
}
