using Core;
using Game.Models;
using System.Linq;
using UltimateSurvival;

namespace Game.Objectives.Actions.Controllers
{
    //TODO: replace with code from InventoryOperationsController
    public class ChoiseItemActionDataController : BaseActionController<ChoiseItemActionData>
    {
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }

        protected override void Action(ChoiseItemActionData actionData)
        {
            var itemData = ItemsDB.ItemDatabase.GetItemByName(actionData.ItemName);
            var cell = HotBarModel.ItemsContainer.Cells.FirstOrDefault(x => x.IsHasItem && x.Item.ItemData.Id == itemData.Id);

            if (cell != null)
            {
                HotBarModel.Equp(cell.Id);
                PlayerEventHandler.ChangeEquippedItem.Try(cell.Item, false);
            }
        }
    }
}
