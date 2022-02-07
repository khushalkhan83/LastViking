using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class DeleteItemController : ViewEnableController<QuestionPopupView>, IDeleteItemController
    {
        [Inject] public DeleteItemModel DeleteItemModel { get; private set; }

        public override ViewConfigID ViewConfigID => ViewConfigID.QuestionPopupConfig;

        public override bool IsCanShow => true;

        private ItemsContainer container;
        private int cellId;


        public override void Enable() 
        {
            DeleteItemModel.OnShowDeletePopup += OnShowDeletePopup;
        }

        public override void Disable() 
        {
            DeleteItemModel.OnShowDeletePopup -= OnShowDeletePopup;
            HideDeletePopup();
        }

        private void OnShowDeletePopup(ItemsContainer container, int cellId)
        {
            this.container = container;
            this.cellId = cellId;
            UpdateViewVisible();
        }

        protected override void OnShow()
        {
            if(View != null)
            {
                View.OnClose += OnCloseTashPopupHandler;
                View.OnApply += OnApplyTrashPopupHandler;
                View.SetTextTitle("Delete Item");
                View.SetTextDescription("Are you sure that you want to delete this item?");
                View.SetTextBackButton("Back");
                View.SetTextOkButton("Delete");
            }
        }

        private void HideDeletePopup()
        {
            if(View != null)
            {
                View.OnClose -= OnCloseTashPopupHandler;
                View.OnApply -= OnApplyTrashPopupHandler;
                Hide();
            }
        }

        private void OnCloseTashPopupHandler() => HideDeletePopup();

        private void OnApplyTrashPopupHandler()
        {
            if(container != null)
            {
                container.ClearCell(cellId);
            }
            HideDeletePopup();
        }

    }
}
