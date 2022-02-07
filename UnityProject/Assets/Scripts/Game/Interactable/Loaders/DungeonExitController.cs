using Game.Models;
using UnityEngine;

namespace Game.Interactables
{
    [RequireComponent(typeof(EnvironmentSceneLoader))]
    public class DungeonExitController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private EnvironmentSceneID _sceneID;
        [SerializeField] private bool _passLocation = true;
        [SerializeField] private bool _resetSavePoint = false;

#pragma warning restore 0649
        #endregion

        private EnvironmentSceneLoader _environmentSceneLoader;
        public DungeonProgressModel DungeonProgressModel => ModelsSystem.Instance._dungeonsProgressModel.GetDungeonProgressModel(_sceneID);

        #region MonoBehaviour

        private void Awake()
        {
            _environmentSceneLoader = GetComponent<EnvironmentSceneLoader>();
        }

        private void OnEnable()
        {
            _environmentSceneLoader.OnPreLoadNextEnvironment += OnPreLoadNextEnvironment;
        }

        private void OnDisable()
        {
            _environmentSceneLoader.OnPreLoadNextEnvironment -= OnPreLoadNextEnvironment;
        }

        #endregion

        private void OnPreLoadNextEnvironment()
        {
            if (_passLocation)
            {
                DungeonProgressModel.PassLocation();
            }
            if (_resetSavePoint)
            {
                DungeonProgressModel.ResetSavePoint();
            }
        }
    }
}