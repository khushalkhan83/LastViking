using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class InfoPanelController : IInfoPanelController, IController
    {
        [Inject] public InfoPanelModel InfoPanelModel { get; private set; }
        [Inject] public InfoPanelToolViewModel InfoPanelToolViewModel { get; private set; }
        [Inject] public InfoPanelViewModel InfoPanelViewModel { get; private set; }
        [Inject] public InfoPanelWeaponViewModel InfoPanelWeaponViewModel { get; private set; }
        [Inject] public InfoPanelFoodViewModel InfoPanelFoodViewModel { get; private set; }
        [Inject] public InfoPanelMedicineViewModel InfoPanelMedicineViewModel { get; private set; }
        [Inject] public InfoPanelDefenceViewModel InfoPanelDefenceViewModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected IView DescriptiuonView { get; private set; }

        void IController.Enable() 
        {
            InfoPanelModel.OnUpdateItemInfo += UpdateItemInfo;
            InfoPanelModel.OnHideItemInfo += HideDescrition;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            InfoPanelModel.OnUpdateItemInfo -= UpdateItemInfo;
            InfoPanelModel.OnHideItemInfo -= HideDescrition;
        }

        private void UpdateItemInfo(ItemData item)
        {
            HideDescrition();
            ShowItemInfo(item);
        }

        private void ShowItemInfo(ItemData item)
        {
            if (item.TryGetProperty("ItemCategory", out var property))
            {
                switch (property.ItemCategoryID)
                {
                    case ItemCategoryID.Defence:
                        ShowInfoPanelDefenceView(item);
                        break;
                    case ItemCategoryID.Medicine:
                        ShowInfoPanelMedicineView(item);
                        break;
                    case ItemCategoryID.Food:
                        ShowInfoPanelFoodView(item);
                        break;
                    case ItemCategoryID.Weapon:
                        ShowInfoPanelWeaponView(item);
                        break;
                    case ItemCategoryID.Tool:
                        ShowInfoPanelToolView(item);
                        break;
                    case ItemCategoryID.Equipment:
                        ShowInfoPanelEquipmentView(item);
                        break;
                }
            }
            else
            {
                ShowInfoPanelView(item);
            }
        }

        private void ShowInfoPanelView(ItemData item)
        {
            InfoPanelViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelViewModel.SetDescription(item.DescriptionKeyID);

            UpdateDesctiption<InfoPanelView>(ViewConfigID.InfoPanel);
        }

        private void ShowInfoPanelToolView(ItemData item)
        {
            InfoPanelToolViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelToolViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelToolViewModel.SetTool(item);

            UpdateDesctiption<InfoPanelToolView>(ViewConfigID.InfoPanelTool);
        }

        private void ShowInfoPanelWeaponView(ItemData item)
        {
            InfoPanelWeaponViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelWeaponViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelWeaponViewModel.SetWeapon(item);

            UpdateDesctiption<InfoPanelWeaponView>(ViewConfigID.InfoPanelWeapon);
        }

        private void ShowInfoPanelFoodView(ItemData item)
        {
            InfoPanelFoodViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelFoodViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelFoodViewModel.SetFoodItem(item);

            UpdateDesctiption<InfoPanelFoodView>(ViewConfigID.InfoPanelFood);
        }

        private void ShowInfoPanelMedicineView(ItemData item)
        {
            InfoPanelMedicineViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelMedicineViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelMedicineViewModel.SetHealth(item);

            UpdateDesctiption<InfoPanelMedicineView>(ViewConfigID.InfoPanelMedicine);
        }

        private void ShowInfoPanelDefenceView(ItemData item)
        {
            InfoPanelDefenceViewModel.SetTitle(item.DisplayNameKeyID);
            InfoPanelDefenceViewModel.SetDescription(item.DescriptionKeyID);
            InfoPanelDefenceViewModel.SetHealthItem(item);

            UpdateDesctiption<InfoPanelDefenceView>(ViewConfigID.InfoPanelDefence);
        }

        private void ShowInfoPanelEquipmentView(ItemData item)
        {
            InfoPanelEquipmentViewData data = new InfoPanelEquipmentViewData(item, true);
            UpdateDesctiption<InfoPanelEquipmentView>(ViewConfigID.InfoPanelEquipmentConfig, data);
        }


        private void UpdateDesctiption<T>(ViewConfigID viewConfigID, IDataViewController data = null) where T : Component, IView
        {
            if (!(DescriptiuonView is T))
            {
                HideDescrition();
                ShowDescrition<T>(viewConfigID, data);
            }
        }

        private void ShowDescrition<T>(ViewConfigID viewConfigID, IDataViewController data = null) where T : Component, IView
        {
            DescriptiuonView = ViewsSystem.Show<T>(viewConfigID, InfoPanelModel.ViewContainer, data);
            ((T)DescriptiuonView).transform.localPosition = Vector3.zero;
        }

        private void HideDescrition()
        {
            if (DescriptiuonView != null)
            {
                ViewsSystem.Hide(DescriptiuonView);
                DescriptiuonView = null;
            }
        }
    }
}
