using Game.Views;
using Core.Controllers;
using Core;
using Game.Models;
using System;

namespace Game.Controllers
{
    public class TaskViewController : ViewControllerBase<TaskView>
    {
        [Inject] public TaskViewModel ViewModel { get; private set; }
        [Inject] public LocalizationModel LocalizationModel { get; private set; }

        // private string Header => LocalizationModel.GetString(LocalizationKeyID.task);

        protected override void Show() 
        {
            UpdateMessage();
            View.PlayShowAnimation(null);
            
            ViewModel.OnMessageChanged += UpdateMessage;
            ViewModel.OnShowFillChanged += OnShowFillChanged;
            ViewModel.OnFillAmountChanged += OnFillAmountChanged;
            ViewModel.OnIconChanged += OnIconChanged;
            ViewModel.OnCountFieldChanged += OnCountFieldChanged;

            // LocalizationModel.OnChangeLanguage += UpdateText;

            UpdateText();
        }

        protected override void Hide() 
        {
            ViewModel.OnMessageChanged -= UpdateMessage;
            ViewModel.OnShowFillChanged -= OnShowFillChanged;
            ViewModel.OnFillAmountChanged -= OnFillAmountChanged;
            ViewModel.OnIconChanged -= OnIconChanged;
            ViewModel.OnCountFieldChanged -= OnCountFieldChanged;
            // LocalizationModel.OnChangeLanguage -= UpdateText;
        }

        private void UpdateText()
        {
            View.Message = ViewModel.Message;
            View.CountField = String.Empty;
        }
        private void OnShowFillChanged() => View.ShowFill(ViewModel.ShowFill);
        private void OnFillAmountChanged() => View.SetFillAmount(ViewModel.FillAmount);
        private void OnIconChanged() => View.SetIcon(ViewModel.Icon);
        private void OnCountFieldChanged() => View.CountField = ViewModel.Count;


        private void UpdateMessage()
        {
            if(View.IsShowing)
            {
                // if(ViewModel.OneShotAnimationTime.HasValue)
                // {
                //     View.SetAnimationTimeOnce(ViewModel.OneShotAnimationTime.Value);
                // }
                
                View.PlayHideAnimation(() =>
                {
                    UpdateView();
                });
            }
            else
            {
                UpdateView();
            }

            void UpdateView()
            {
                // if(ViewModel.OneShotAnimationTime.HasValue)
                // {
                //     View.SetAnimationTimeOnce(ViewModel.OneShotAnimationTime.Value);
                // }
                
                UpdateText();
                View.SetFillAmount(0, false);
                View.PlayShowAnimation();
                // ViewModel.ClearOneShotAnimationTime();
            }
        }
    }
}
