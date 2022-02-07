using Game.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Intro
{
    public class IntroLoadingController : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private IntroLoadingModel introModel;
        [SerializeField] private int introSceneIndex;
        [SerializeField] private int loadingSceneIndex;

    #pragma warning restore 0649
        #endregion

        #region MonoBehaviour

        private void OnEnable()
        {
            introModel.InitData();
            if(!introModel._Data.introPlayed)
            {
                LoadIntro();
            }
            else
            {
                LoadGame();
            }
        }

        #endregion

        private void LoadIntro()
        {
            SceneManager.LoadScene(introSceneIndex);
        }

        private void LoadGame()
        {
            SceneManager.LoadScene(loadingSceneIndex);
        }
    }
}