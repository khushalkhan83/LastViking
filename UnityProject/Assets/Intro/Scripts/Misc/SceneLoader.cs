using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.Misc
{
    public class SceneLoader : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private int sceneIndex;
        [SerializeField] private bool loadOnEnable;
        
        #pragma warning restore 0649
        #endregion

        #region MonoBehaviour
        private void OnEnable()
        {
            if(!loadOnEnable) return;

            SceneManager.LoadScene(sceneIndex);
        }
        #endregion
    }
}