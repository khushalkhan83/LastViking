using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class LoadingSlider : MonoBehaviour
    {
        AsyncOperation async;

        public Slider loadingSlider;

        void Start()
        {
            async = SceneManager.LoadSceneAsync(0, LoadSceneMode.Single);
        }

        void Update()
        {
            loadingSlider.value = async.progress;
        }
    }
}