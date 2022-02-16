using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class FullInventoryController : IFullInventoryController, IController
    {
        [Inject] public FullInventoryModel FullInventoryModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ExpandInventoryModel ExpandInventoryModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public TutorialSimpleDarkViewModel TutorialSimpleDarkViewModel { get; private set; }

        protected InventoryIsFillPopupView InventoryIsFillPopupView { get; private set; }

        void IController.Enable() 
        {
            FullInventoryModel.OnShowFullPopup += ShowFullPopup;
            FullInventoryModel.OnHideFullPopup += HideFullPopup;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            FullInventoryModel.OnShowFullPopup -= ShowFullPopup;
            FullInventoryModel.OnHideFullPopup -= HideFullPopup;
        }

        private void ShowFullPopup()
        {
            if(TutorialModel.IsTutorialNow)
            {
                TutorialSimpleDarkViewModel.SetShow(true);
            }
            InventoryIsFillPopupView = ViewsSystem.Show<InventoryIsFillPopupView>(ViewConfigID.InventoryIsFillPopup);
            InventoryIsFillPopupView.OnBack += OnBack;
            InventoryIsFillPopupView.OnExpand += OnExpandInventory;
        }

        private void HideFullPopup()
        {
            if(TutorialModel.IsTutorialNow)
            {
                TutorialSimpleDarkViewModel.SetShow(false);
            }
            InventoryIsFillPopupView.OnBack -= OnBack;
            InventoryIsFillPopupView.OnExpand -= OnExpandInventory;
            ViewsSystem.Hide(InventoryIsFillPopupView);
            InventoryIsFillPopupView = null;
        }

        private void OnExpandInventory()
        {
            ExpandInventoryModel.ExpandInventory();
            HideFullPopup();
        }

        private void OnBack()
        {
            ExpandInventoryModel.TriggerNotExpandedCallback();
            HideFullPopup();
        }
    }
}
