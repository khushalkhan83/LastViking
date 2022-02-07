using System.Collections;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class ConstructionInfoController : IConstructionInfoController, IController
    {
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public ConstructionInfoModel ConstructionInfoModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }

        private int _coroutineNum = -1;

        public void Enable()
        {
            return;
            if (ConstructionInfoModel.IsConstructionInfoShown) return;

            if (TutorialModel.IsComplete)
            {
                OnTutorialComplete();
            }
            else 
            {
                TutorialModel.OnComplete += OnTutorialComplete;
            }
        }

        public void Disable()
        {
            TutorialModel.OnComplete -= OnTutorialComplete;
            CoroutineModel.BreakeCoroutine(_coroutineNum);
        }

        public void Start()
        {
         
        }

        private void OnTutorialComplete()
        {
            _coroutineNum = CoroutineModel.InitCoroutine(WaitForShowView());
        }

        private IEnumerator WaitForShowView()
        {
            yield return new WaitForSeconds(ConstructionInfoModel.ShowConstructionInfoDelay);
            while (true)
            {
                yield return new WaitForSeconds(2f);
                if (!IsWindowOrPopupOpen())
                {
                    if (!ConstructionInfoModel.IsConstructionInfoShown)
                    {
                        ViewsSystem.Show<ConstructionInfoPopupView>(ViewConfigID.ConstructionInfoPopupView);
                        ConstructionInfoModel.ShowConstructionInfo();
                    }
                    break;
                }
            }
        }

        private bool IsWindowOrPopupOpen()
        {
            foreach (var views in ViewsSystem.ActiveViews.Values)
            {
                foreach (var view in views)
                {
                    if (ViewsSystem.Configs.TryGetValue(view, out var configData))
                    {
                        if (ConstructionInfoModel.CanShowOverViews.Contains(configData.ViewID))
                        {
                            continue;
                        }

                        LayerID id = configData.LayerID;
                        if (id == LayerID.Window || id == LayerID.Popup)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }
}
