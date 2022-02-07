using Core;
using Game.Models;

namespace Game.Controllers
{
    public class CoastLootPopupViewController : TreasureLootPopupViewController
    {
        [Inject] public CoastLootChestModel CoastLootChestModel { get; set; }

        protected override void OnChestOpen()
        {
            CoastLootChestModel.OnChestOpen();
        }
    }
}