using Game.Models;

namespace Game.Controllers
{
    public class TutorialOutlineMineableStoneController : TutorialOutlineMineableControllerBase, ITutorialOutlineMineableStoneController
    {
        protected override OutLineMinableObjectID outLineMinableObjectID => OutLineMinableObjectID.Stone;

    }
}
