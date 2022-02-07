using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;

namespace Game.Controllers
{
    public class AddingInventoryItemsController : IAddingInventoryItemsController, IController
    {
        [Inject] public InventoryModel InventoryModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        [Inject] public AddingInventoryItemsModel AddingInventoryItemsModel { get; private set; }

        void IController.Enable()
        {
           
        }

        void IController.Start()
        {
            
        }

        void IController.Disable()
        {
           
        }

        
    }
}
