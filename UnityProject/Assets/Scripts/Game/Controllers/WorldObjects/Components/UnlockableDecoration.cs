using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class UnlockableDecoration : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private int _medalionsNeeded;
        [SerializeField] private GameObject _view;
        #pragma warning restore 0649
        #endregion

        private MedallionsModel MedallionsModel => ModelsSystem.Instance._medallionsModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        private bool environmentLoaded;

        #region MonoBehaviour

        private void Awake()
        {
            _view.SetActive(false);
        }

        private void OnEnable()
        {
            MedallionsModel.OnCollect += RefreshView;
            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;

            RefreshView();
        }

        private void OnDisable()
        {
            PlayerScenesModel.OnEnvironmentLoaded -= RefreshView;
            MedallionsModel.OnCollect -= RefreshView;
        }

        #endregion

        private void OnEnvironmentLoaded()
        {
            environmentLoaded = true;
            RefreshView();
        }

        private void RefreshView()
        {
            if(!environmentLoaded) return;

            bool show = MedallionsModel.Collected >= _medalionsNeeded;
            _view.SetActive(show);
        }
    }
}