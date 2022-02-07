using Game.Views;
using Core.Controllers;
using System;
using Core;
using Game.Models;

namespace Game.Controllers
{
    public class LearnInputViewController : ViewControllerBase<LearnInputView>
    {
        [Inject] public LearnInputViewModel ViewModel { get; private set; }

        protected override void Show() 
        {
            ViewModel.MaxStep = View.Steps.Count;

            ViewModel.OnStepChanged += UpdateView;
            View.OnClick += ProcessClick;

            UpdateView();
        }

        protected override void Hide() 
        {
            ViewModel.OnStepChanged -= UpdateView;
            View.OnClick -= ProcessClick;
        }

        private void ProcessClick()
        {
            if(ViewModel.IsLastStep)
            {
                ViewModel.SetFinish();
            }
            else
            {
                ViewModel.SetStep(ViewModel.Step + 1);
            }
        }

        private void UpdateView()
        {
            View.Steps.ForEach(x => x.SetActive(false));
            var index = ViewModel.Step - 1;
            View.Steps[index].SetActive(true);
        }
    }
}
