using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class ItemsOwnershipController : IItemsOwnershipController, IController
    {
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public ItemsOwnershipModel ItemsOwnershipModel { get; private set; }

        void IController.Enable()
        {
            InventoryModel.ItemsContainer.OnChangeCell += OnChangeCell;
            InventoryModel.ItemsContainer.OnPreAddItems += OnPreAddItems;
            HotBarModel.ItemsContainer.OnChangeCell += OnChangeCell;
            HotBarModel.ItemsContainer.OnPreAddItems += OnPreAddItems;
        }

        void IController.Start()
        {
            foreach (var c in InventoryModel.ItemsContainer.Cells)
            {
                if (c.IsHasItem)
                    SetBelongToPlayer(c.Item);
            }
            foreach (var c in HotBarModel.ItemsContainer.Cells)
            {
                if (c.IsHasItem)
                    SetBelongToPlayer(c.Item);
            }
        }

        void IController.Disable()
        {
            InventoryModel.ItemsContainer.OnChangeCell -= OnChangeCell;
            HotBarModel.ItemsContainer.OnChangeCell -= OnChangeCell;
            InventoryModel.ItemsContainer.OnPreAddItems -= OnPreAddItems;
            HotBarModel.ItemsContainer.OnPreAddItems -= OnPreAddItems;
        }

        private void OnChangeCell(CellModel cell)
        {
            if (cell.IsHasItem)
            {
                CheckBelongToPlayer(cell.Item, cell.Item.Count);
                SetBelongToPlayer(cell.Item);
            }
        }

        private void OnPreAddItems(SavableItem item, int count)
        {
            CheckBelongToPlayer(item, item.Count);
            SetBelongToPlayer(item);
        }

        private void CheckBelongToPlayer(SavableItem item, int count)
        {
            if (!item.HasProperty("BelongsToPlayer"))
            {
                ItemsOwnershipModel.ItemOwnedByPlayer(item.Id, item.Count);
            }
        }

        private void SetBelongToPlayer(SavableItem item)
        {
            if (!item.HasProperty("BelongsToPlayer"))
            {
                var prop = new ItemProperty.Value();
                prop.SetName("BelongsToPlayer");
                item.CurrentPropertyValues.Add(prop);
            }
        }
    }
}
