using System.Collections;
using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class PerformanceStartCheckController : IController, IPerformanceStartCheckController
    {
        [Inject] public PerformanceStartCheckModel PerformanceStartCheckModel { get; private set; }
        [Inject] public CoroutineModel CoroutineModel { get; private set; }
        [Inject] public QualityModel QualityModel { get; private set; }
        [Inject] public PlayerScenesModel PlayerScenesModel { get; private set; }
        [Inject] public TutorialModel TutorialModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        private int coroutineId = -1;

        public void Enable() 
        {
            if(!TutorialModel.IsComplete && !PerformanceStartCheckModel.PerformanceChecked)
            {
                ViewsSystem.OnEndHide.AddListener(ViewConfigID.TutorialDark, OnHideTutorialDarkScreen);
            }
        }

        public void Start() 
        {
        }

        public void Disable() 
        {
            ViewsSystem.OnEndHide.RemoveListener(ViewConfigID.TutorialDark, OnHideTutorialDarkScreen);
            CoroutineModel.BreakeCoroutine(coroutineId);
        }

        private void OnHideTutorialDarkScreen()
        {
            coroutineId = CoroutineModel.InitCoroutine(CheckCoroutine());
        }

        private IEnumerator CheckCoroutine()
        {
            yield return new WaitForSeconds(PerformanceStartCheckModel.StartDelay);
            int lowFPSCount = 0;
            for(int i = 0; i < 8; i++)
            {
                if (LowFPS())
                    lowFPSCount++;
                else
                    lowFPSCount--;
                yield return new WaitForSeconds(0.2f);
            }

            if (lowFPSCount >= 0)
            {
                QualityModel.DecreaseQuality();
            }

            PerformanceStartCheckModel.PerformanceChecked = true;
        }

        private bool LowFPS() => GetFPS() <= PerformanceStartCheckModel.LowFPS;

        private static int GetFPS() => (int)(1.0f / Time.smoothDeltaTime);

    }
}
