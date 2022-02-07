using System.Collections;
using System.Collections.Generic;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;


namespace Game.Controllers
{
    public class LowPerformanceViewController : ViewControllerBase<QuestionPopupView>
    {
        [Inject] public ViewsSystem ViewsSystem {get;set;}
        [Inject] public QualityModel QualityModel {get;set;}
        [Inject] public LocalizationModel LocalizationModel {get;set;}

        protected override void Show()
        {
            View.OnApply += OnApply;
            View.OnClose += OnClose;
            LocalizationModel.OnChangeLanguage += SetLocalization;
            SetLocalization();
        }

        protected override void Hide()
        {
            View.OnApply -= OnApply;
            View.OnClose -= OnClose;
            LocalizationModel.OnChangeLanguage -= SetLocalization;
        }

        private void OnApply()
        {
            QualityModel.DecreaseQuality();
            ViewsSystem.Hide(View);
        }

        private void OnClose()
        {
            ViewsSystem.Hide(View);
        }


        private void SetLocalization()
        {
            View.SetTextTitle(LocalizationModel.GetString(LocalizationKeyID.LowPerformance_Title));
            View.SetTextDescription(LocalizationModel.GetString(LocalizationKeyID.LowPerformance_Description));
            View.SetTextOkButton(LocalizationModel.GetString(LocalizationKeyID.ResetWarning_OkBtn));
            View.SetTextBackButton(LocalizationModel.GetString(LocalizationKeyID.NotEnoughSpacePopUp_BackBtn));
        }
    }
}
