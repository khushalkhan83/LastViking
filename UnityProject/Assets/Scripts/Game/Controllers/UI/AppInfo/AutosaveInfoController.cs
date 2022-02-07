using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;
using System.Collections;
using UnityEngine;

namespace Game.Controllers
{
    public class AutosaveInfoController : IAutosaveInfoController, IController
    {
        [Inject] public AutosaveModel AutosaveModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        public IView AutoSaveInfoView { get; private set; }

        private int _showSaveViewCoroutineId = -1;

        void IController.Enable()
        {
            AutosaveModel.OnSave += OnSaveAll;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            AutosaveModel.OnSave -= OnSaveAll;
            CoroutineModel.BreakeCoroutine(_showSaveViewCoroutineId);
        }

        private void OnSaveAll()
        {
            if (AutoSaveInfoView == null)
            {
                _showSaveViewCoroutineId = CoroutineModel.InitCoroutine(ShowSaveView());
            }
        }

        private IEnumerator ShowSaveView()
        {
            AutoSaveInfoView = ViewsSystem.Show<AutoSaveInfoView>(ViewConfigID.AutoSaveInfo);
           
            yield return new WaitForSecondsRealtime(3);
           
            ViewsSystem.Hide(AutoSaveInfoView);
            AutoSaveInfoView = null;
        }
    }
}
