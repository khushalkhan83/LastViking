using Game.Models;
using UnityEngine;

namespace Game.Controllers
{
    public class MainTargetController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private MainTargetView _mainTargetView;
        [SerializeField] private  MainTargetModel _mainTargetModel;

#pragma warning restore 0649
        #endregion

        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;

        public MainTargetModel MainTargetModel => _mainTargetModel;

        public MainTargetView MainTargetView => _mainTargetView;

        private void Start()
        {
            if (SheltersModel.ShelterActive != ShelterModelID.None)
            {
                UpdatePosition();
            }
        }

        private void OnEnable()
        {
            if (SheltersModel.ShelterActive == ShelterModelID.None)
            {
                MainTargetModel.OnChangePosition += OnChangePositionHandler;
            }

            SheltersModel.OnActivate += OnActivateHandler;
        }

        private void OnDisable()
        {
            SheltersModel.OnActivate -= OnActivateHandler;
        }

        private void OnChangePositionHandler()
        {
            UpdatePosition();
        }

        private void OnActivateHandler(ShelterModel shelterModel)
        {
            MainTargetModel.SetPosition(shelterModel.CorePosition);
        }

        private void UpdatePosition()
        {
            MainTargetView.SetPosition(MainTargetModel.Position);
        }
    }
}
