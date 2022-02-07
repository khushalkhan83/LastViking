using UnityEngine;

namespace Game.ThirdPerson.HUD
{
    public class HUDViewController : MonoBehaviour
    {
        private HUDViewModel viewModel;
        private HUDView view;

        private void Awake()
        {
            viewModel = GetComponent<HUDViewModel>();
            view = GetComponent<HUDView>();
        }

        private void OnEnable()
        {
            viewModel.OnShowChanged += UpdateView;
            viewModel.OnShowAimButtonChanged += UpdateView;

            UpdateView();
        }

        private void OnDisable()
        {
            viewModel.OnShowChanged -= UpdateView;
            viewModel.OnShowAimButtonChanged -= UpdateView;

            view.Show(false);
        }

        private void UpdateView()
        {
            view.Show(viewModel.IsShow);
            view.ShowAimButton(viewModel.IsShowAimButton);
        }
    }
}