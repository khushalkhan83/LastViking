using static UltimateSurvival.FPTool;

namespace Game.Controllers
{
    public class TutorialRocksExtractionController : ResourcesExtractionController, ITutorialRocksExtractionController
    {
        protected override ToolPurpose toolPurpose => ToolPurpose.BreakRocks;
        protected override int modifier => TutorialResourcesModel.RocksExtractionModifier;
    }
}
