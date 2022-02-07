using Game.Models;
using Game.Views;
using SimpleDiskUtils;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Controllers
{
    public class StartupController : MonoBehaviour
    {
        private const int k_loadingSceneIndex = 2;
        private const int k_coreSceneIndex = 3;
        
        #region Data
#pragma warning disable 0649

        [SerializeField] private float _timeFade;
        [SerializeField] private float _delayBeforeFade;

        [SerializeField] private EnoughSpaceModel _enoughSpaceModel;

#pragma warning restore 0649
        #endregion

        private StartupView _startupView;
        public StartupView StartupView => _startupView ?? (_startupView = FindObjectOfType<StartupView>());
        private EnoughSpaceModel EnoughSpaceModel => _enoughSpaceModel;

        public float TimeFade => _timeFade;
        public float DelayBeforeFade => _delayBeforeFade;


        #region Monobehaviour
            
        private void Start()
        {
            StartCoroutine(LoadingProcess());
        }

        #endregion

        IEnumerator LoadingProcess()
        {
            yield return null;

            yield return null;

            bool notEnoughSpace = !EnoughSpaceModel.HasEnoughSpace(out bool critical);
            bool criticalyNotEnoughSpace = notEnoughSpace && critical;

            if(criticalyNotEnoughSpace) yield break;

            yield return StartCoroutine(LoadCoreProcess());

            yield return new WaitForSecondsRealtime(DelayBeforeFade);
            yield return StartCoroutine(FadeProcess(TimeFade));
            yield return null;
            SceneManager.UnloadSceneAsync(k_loadingSceneIndex);
        }

        //Load core process
        IEnumerator LoadCoreProcess()
        {
            var loadMainAsync = SceneManager.LoadSceneAsync(k_coreSceneIndex, LoadSceneMode.Additive); 

            do
            {
                StartupView.SetSliderValue(GetProgressNormalized(loadMainAsync));
                StartupView.SetSliderText((loadMainAsync.progress).ToString("P"));
                   
                yield return null;
            }
            while (!loadMainAsync.isDone);

            yield return null;
        }

        private float GetProgressNormalized(AsyncOperation loadMainAsync) => Mathf.Clamp01(loadMainAsync.progress / 0.9f);

        //Canvas fade process
        IEnumerator FadeProcess(float time)
        {
            var timeRemaining = time;
            while (timeRemaining > 0)
            {
                yield return null;

                timeRemaining -= Time.unscaledDeltaTime;
                StartupView.SetAlpha(Mathf.Clamp01(timeRemaining / time));
            }
        }
    }
}