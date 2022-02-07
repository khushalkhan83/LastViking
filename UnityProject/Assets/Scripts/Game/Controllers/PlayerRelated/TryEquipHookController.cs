namespace Game.Controllers
{
    public class TryEquipHookController : TryEquipItemController, ITryEquipHookController
    {
        protected override string ItemName => "tool_hook";
    }

}