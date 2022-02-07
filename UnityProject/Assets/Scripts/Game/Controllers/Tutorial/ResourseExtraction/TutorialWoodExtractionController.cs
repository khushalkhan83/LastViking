using Core;
using Game.Models;
using static UltimateSurvival.FPTool;

namespace Game.Controllers
{
    public class TutorialWoodExtractionController : ResourcesExtractionController, ITutorialRocksExtractionController
    {
        protected override ToolPurpose toolPurpose => ToolPurpose.CutWood;
        protected override int modifier => TutorialResourcesModel.WoodExtractionModifier;
    }
}
