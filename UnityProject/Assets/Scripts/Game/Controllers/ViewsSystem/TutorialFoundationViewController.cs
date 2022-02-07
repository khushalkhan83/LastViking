using Game.Views;
using Core.Controllers;
using Game.Models;
using Core;
using UnityEngine;

namespace Game.Controllers
{
    public class TutorialFoundationViewController : ViewControllerBase<TutorialFoundationView>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ConstructionTutorialModel ConstructionTutorialModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }
        [Inject] public BuildingModeModel BuildingModeModel { get; private set; }
        [Inject] public BuildingHotBarModel BuildingHotBarModel { get; private set; }
        [Inject] public TutorialHouseModel TutorialHouseModel { get; private set; }

        private readonly float clickDelay = 0.5f;

        protected override void Show()
        {
            BuildingModeModel.BuildingActive = true;
            View.transform.SetAsLastSibling();
            DisableClick();
            LocalizationModel.OnChangeLanguage += SetLocalization;

            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnClick -= OnClick;
            GameUpdateModel.OnUpdate -= OnUpdate;
            LocalizationModel.OnChangeLanguage -= SetLocalization;
        }

        private float _clickTimer = 0;
        private void OnUpdate()
        {
            _clickTimer += Time.deltaTime;
            if (_clickTimer >= clickDelay)
            {
                _clickTimer = 0;
                EnableClick();
            }
        }

        private void OnClick()
        {
            //TODO selected cell
            BuildingHotBarModel.SelectedCell = TutorialHouseModel.SellectedCellFoundation;
            BuildingModeModel.BuildingActive = false;
            BuildingModeModel.BuildingActive = true;
            ViewsSystem.Hide(View);
        }

        private void EnableClick()
        {
            View.OnClick += OnClick;
            GameUpdateModel.OnUpdate -= OnUpdate;
        }

        private void DisableClick()
        {
            View.OnClick -= OnClick;
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        private void SetLocalization()
        {
            View.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.Tutorial_UI_UseConstructionHotbar)); 
        }
    }
}
