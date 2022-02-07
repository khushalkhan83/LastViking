namespace Game.Controllers
{
    public class UseCoconutTutorialController : UseItemTutorialController, IUseCoconutTutorialController
    {
        protected override string TargetItemName => "food_coconut";
    }
}