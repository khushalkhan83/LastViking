using Game.Models;
using Game.Providers;
using Game.Views;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class ShelterDefaultViewController : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ShelterModelID _shelterModelID;

#pragma warning restore 0649
        #endregion

        private ShelterModelID ShelterModelID => _shelterModelID;
        private ViewsSystem ViewsSystem => ViewsSystem.Instance;
        private SheltersModel SheltersModel => ModelsSystem.Instance._sheltersModel;
        private ShelterModelsProvider _shelterModelsProvider;
        private ShelterModelsProvider ShelterModels => ModelsSystem.Instance._shelterModelsProvider;

        public ShelterModel ShelterModel => ShelterModels[ShelterModelID];

        public ShelterCursorBuyView ShelterCursorView { get; private set; }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.GetComponent<PlayerEventHandler>())
            {
                return;
            }

            ShowCursor();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<PlayerEventHandler>())
            {
                return;
            }

            HideCursor();
        }

        private void OnEnable()
        {
            ShelterModel.OnBuy += OnBuyHandler;
        }

        private void OnDisable()
        {
            ShelterModel.OnBuy -= OnBuyHandler;
            HideCursor();
        }

        private void ShowCursor()
        {
            if (!ShelterCursorView)
            {
                SheltersModel.SetShelter(ShelterModel);
                ShelterCursorView = ViewsSystem.Show<ShelterCursorBuyView>(ViewConfigID.ShelterCursorBuy);

                ShelterCursorView.OnDownPoint += OnDownPointHandler;
            }
        }

        private void HideCursor()
        {
            if (ShelterCursorView)
            {
                ShelterCursorView.OnDownPoint -= OnDownPointHandler;

                ViewsSystem.Hide(ShelterCursorView);
                ShelterCursorView = null;
            }
        }

        private void OnDownPointHandler()
        {
            ViewsSystem.Show<ShelterPopupView>(ViewConfigID.ShelterPopup);
        }

        private void OnBuyHandler()
        {
            HideCursor();
        }
    }
}
