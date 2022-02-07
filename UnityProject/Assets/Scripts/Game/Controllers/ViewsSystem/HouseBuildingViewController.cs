using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;
using System;
using Extensions;
using Game.Components;
using UnityEngine;

namespace Game.Controllers
{
    public class HouseBuildingViewController : ViewControllerBase<HouseBuildingView, HouseBuildingViewControllerData>
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public HouseBuildingViewModel ViewModel { get; private set; }
        [Inject] public DragItemViewModel DragItemViewModel { get; private set; }

        protected override void Show() 
        {
            GameUpdateModel.OnUpdate += UpdateTimer;
            View.OnCloseButtonClick += OnCloseButtonClick;
            View.OnBuildButtonClick += OnBuildButtonClick;
            View.OnSkipButtonClick += OnSkipButtonClick;
            View.OnRestoreButtonClick += OnRestoreButtonClick;
            ViewModel.OnSkipButtonHilightChanged += UpdateTutorialHilight;
            ViewModel.OnIsBuildButtonHilightChanged += UpdateTutorialHilight;
            Data.OnViewDataChanged += UpdateView;

            UpdateView();
        }

        protected override void Hide() 
        {
            GameUpdateModel.OnUpdate -= UpdateTimer;

            View.OnCloseButtonClick -= OnCloseButtonClick;
            View.OnBuildButtonClick -= OnBuildButtonClick;
            View.OnSkipButtonClick -= OnSkipButtonClick;
            View.OnRestoreButtonClick -= OnRestoreButtonClick;
            ViewModel.OnSkipButtonHilightChanged -= UpdateTutorialHilight;
            ViewModel.OnIsBuildButtonHilightChanged -= UpdateTutorialHilight;
            Data.OnViewDataChanged -= UpdateView;
        }

        private void OnCloseButtonClick() => Data.userInput.closeViewRequest();
        private void OnBuildButtonClick() {Data.userInput.buildRequest(); ViewModel.BuildClick(); }  // TODO: move to controller
        private void OnSkipButtonClick() {Data.userInput.skipBuildRequest(); ViewModel.SkipClick();}
        private void OnRestoreButtonClick() => Data.userInput.restoreRequest();


        private void UpdateView()
        {
            var vd = Data.ViewData;

            View.BuildButtonObject.SetActive(vd.showBuildButton);
            View.RestoreButton.SetActive(vd.showRestoreButton);
            View.TimerText.gameObject.SetActive(vd.showTimer);
            View.MainIcon.sprite = vd.icon;
            View.CitizensCountText.text = vd.citizensCountText;
            View.HeaderText.text = vd.headerText;
            View.DescriptionText.text = vd.headerText;

            UpdateRequiredItems(vd);
            UpdateRequiredCitizens(vd);
            UpdateRequiredBuildings(vd);
            UpdateBuildingHealth(vd);
            UpdateSkipPrice(vd);
        }

        private void UpdateSkipPrice(HouseViewData vd)
        {
            View.SkipButtonObject.SetActive(vd.showSkipButton);
            View.SkipCoinsText.text = vd.coinsCostPriceText;
        }

        private void UpdateRequiredItems(HouseViewData vd)
        {
            int i = 0;

            if(vd.requiaredItemsViewDatas != null)
            {
                for(; i < vd.requiaredItemsViewDatas.Length && i < View.ResourceCells.Length; i++)
                    {
                        View.ResourceCells[i].gameObject.SetActive(true);
                        View.ResourceCells[i].SetData(vd.requiaredItemsViewDatas[i].cellData);
                    }
            }

            for(; i < View.ResourceCells.Length; i++)
            {
                View.ResourceCells[i].gameObject.SetActive(false);
            }
        }

        private void UpdateRequiredCitizens(HouseViewData vd)
        {
            bool showCitizens = vd.showCitizensResources;
            if(showCitizens)
                View.CitizensCell.SetData(vd.citizensViewData.resourceCellData);

            View.CitizensCell.gameObject.SetActive(showCitizens);
        }

        private void UpdateBuildingHealth(HouseViewData vd)
        {
            View.HealthBar.SetActive(vd.showHealthBar);
            View.HealthFillImage.fillAmount = vd.health / vd.healthMax;
            View.BuildingHealthText.text = $"{vd.health}/{vd.healthMax}";
        }

        private void UpdateRequiredBuildings(HouseViewData vd)
        {
            if (vd.showRequiredBuildings)
            {
                
                int i = 0;

                if(vd.requiaredItemsViewDatas != null)
                {
                    for (; i < vd.requiredBuildingsViewDatas.Length && i < View.BuildingCells.Length; i++)
                    {
                        View.BuildingCells[i].gameObject.SetActive(true);
                        View.BuildingCells[i].SetData(vd.requiredBuildingsViewDatas[i].cellData);
                    }
                }

                for (; i < View.BuildingCells.Length; i++)
                {
                    View.BuildingCells[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for (int i = 0; i < View.BuildingCells.Length; i++)
                {
                    View.BuildingCells[i].gameObject.SetActive(false);
                }
            }
        }
        private void UpdateTimer()
        {
            var request = Data.buildingUpgradeTimerRequets();
            if(request.success)
            {
                View.TimerText.text = request.result;
            }
        }

        #region Tutorial
        private void UpdateTutorialHilight()
        {
            UpdateButtonHilight(View.SkipButtonObject,ViewModel.IsSkipButtonHilight);
            UpdateButtonHilight(View.BuildButtonObject,ViewModel.IsBuildButtonHilight);
        }

        private void UpdateButtonHilight(GameObject button, bool isHilight)
        {
            if(isHilight)
            {
                button.SafeActivateComponent<TutorialHilightAndAnimation>();
            }
            else
            {
                button.SafeDeactivateComponent<TutorialHilightAndAnimation>();
            }
        }
        #endregion
    }
}
