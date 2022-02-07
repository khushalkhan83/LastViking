using Core;
using Game.Models;

namespace Game.Objectives.Actions.Controllers
{
    public class RewardGoldActionController : BaseActionController<RewardGoldActionData>
    {
        [Inject] public CoinsModel CoinsModel { get; private set; }

        protected override void Action(RewardGoldActionData actionData)
        {
            CoinsModel.Adjust(actionData.GoldReward);
        }
    }
}
