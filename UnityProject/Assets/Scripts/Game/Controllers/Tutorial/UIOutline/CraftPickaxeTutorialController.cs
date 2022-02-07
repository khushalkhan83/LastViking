namespace Game.Controllers
{
    public class CraftPickaxeTutorialController : CraftItemTutorialController, ICraftPickaxeTutorialController
    {
        protected override string TargetItemName => "tool_pickaxe_stone";
        protected override bool TeachUseBoost => true;
    }
}
