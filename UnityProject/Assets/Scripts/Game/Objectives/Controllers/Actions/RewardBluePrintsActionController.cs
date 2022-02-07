using Core;
using Game.Models;

namespace Game.Objectives.Actions.Controllers
{
    public class RewardBluePrintsActionController : BaseActionController<RewardBluePrintsActionData>
    {
        [Inject] public BluePrintsModel BluePrintsModel { get; private set; }

        protected override void Action(RewardBluePrintsActionData actionData)
        {
            BluePrintsModel.Adjust(actionData.BluePrintsReward);
        }
    }
}
