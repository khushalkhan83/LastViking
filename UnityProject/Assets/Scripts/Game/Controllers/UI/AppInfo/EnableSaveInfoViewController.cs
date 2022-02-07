using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class EnableSaveInfoViewController : IEnableSaveInfoViewController, IController
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public AutosaveModel AutosaveModel { get; private set; }

        private SaveInfoView SaveInfoView { get;set;}

        void IController.Enable()
        {
            AutosaveModel.OnSave += OnSaveHandler;
        }

        void IController.Start()
        {
        }

        void IController.Disable()
        {
            AutosaveModel.OnSave -= OnSaveHandler;
            HideInfoView();
        }

        void OnSaveHandler()
        {
            AutosaveModel.OnSave -= OnSaveHandler;
            UpdateView();
            AutosaveModel.OnSave += OnSaveHandler;
        }

        void UpdateView()
        {
            if (SaveInfoView != null)
            {
                HideInfoView();
                SaveInfoView = ViewsSystem.Show<SaveInfoView>(ViewConfigID.SaveInfo);
            }
            else
            {
                SaveInfoView = ViewsSystem.Show<SaveInfoView>(ViewConfigID.SaveInfo);
            }
        }

        void HideInfoView()
        {
            if (SaveInfoView != null)
            {
                ViewsSystem.Hide(SaveInfoView);
            }
        }
    }
}
