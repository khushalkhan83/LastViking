namespace Game.Controllers
{
    public class UseBananaTutorialController : UseItemTutorialController, IUseBananaTutorialController
    {
        protected override string TargetItemName => "food_banane";
    }
}