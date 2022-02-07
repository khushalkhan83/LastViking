using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public abstract class TryEquipItemController : ITryEquipHookController, IController
    {
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        protected abstract string ItemName {get;}
        void IController.Enable() 
        {
            InventoryOperationsModel.TryEquipItem(ItemName);
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
        }

    }
}