using Core;
using Core.Controllers;
using Game.Audio;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class DivideItemsController : IDivideItemsController, IController
    {
        [Inject] public DivideItemsModel DivideItemsModel { get; private set; }
        [Inject] public AudioSystem AudioSystem { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        protected DividePopupView DividePopupView { get; private set; }
        protected ItemsContainer Container { get; private set; }
        protected CellModel Cell { get; private set; }

        void IController.Enable() 
        {
            DivideItemsModel.OnShowDividePopup += ShowDividePopup;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            DivideItemsModel.OnShowDividePopup -= ShowDividePopup;
        }

        private void ShowDividePopup(ItemsContainer container, CellModel cell)
        {
            Container = container;
            Cell = cell;

            AudioSystem.PlayOnce(AudioID.WindowOpen);
            DividePopupView = ViewsSystem.Show<DividePopupView>(ViewConfigID.DividePopup);
            SetLocalizationDividePopup();

            var halfCount = cell.Item.Count / 2;

            DividePopupView.SetCountItems(cell.Item.Count);
            DividePopupView.SetValue(halfCount);
            DividePopupView.SetIconItem(cell.Item.ItemData.Icon);
            DividePopupView.ValueChange();

            DividePopupView.OnClose += HideDividePopup;
            DividePopupView.OnApply += OnApplyDivide;
        }

        private void HideDividePopup()
        {
            if (DividePopupView != null)
            {
                DividePopupView.OnApply -= OnApplyDivide;
                DividePopupView.OnClose -= HideDividePopup;
                ViewsSystem.Hide(DividePopupView);
                DividePopupView = null;
            }
            DivideItemsModel.HideDividePopup();
        }

        private void OnApplyDivide()
        {
            var itemSplit = Container.SplitItems(Cell.Id, DividePopupView.ValueRigth);
            DivideItemsModel.ItemSplitted(itemSplit);
            HideDividePopup();
        }

        private void SetLocalizationDividePopup()
        {
            DividePopupView?.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.DivideMenu_Name));
            DividePopupView?.SetTextApplyButton(LocalizationModel.GetString(LocalizationKeyID.DivideMenu_ApplyBtn));
        }
    }
}
