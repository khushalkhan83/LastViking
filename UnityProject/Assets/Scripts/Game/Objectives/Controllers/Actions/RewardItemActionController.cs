using Core;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Objectives.Actions.Controllers
{
    public class RewardItemActionController : BaseActionController<RewardItemActionData>
    {
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        protected override void Action(RewardItemActionData actionData)
        {
            InventoryOperationsModel.AddItemToPlayer(actionData.ItemName, actionData.ItemCount);
        }
    }
}
