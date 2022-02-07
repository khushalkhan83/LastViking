using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;
using EasyBuildSystem.Runtimes.Internal.Socket;
using Game.Models;

namespace Game.Controllers
{
    public class DockController : MonoBehaviour
    {
        #region Data
    #pragma warning disable 0649

        [SerializeField] PartBehaviour _partBehaviour;
        [SerializeField] GameObject _ghostPreview;
        [SerializeField] GameObject _dockObject;
        [SerializeField] GameObject _upgradeProcessPreview;
        [SerializeField] GameObject _dockConstructedCinematic;

    #pragma warning restore 0649
        #endregion

        private ConstructionDockModel ConstructionDockModel => ModelsSystem.Instance._constructionDockModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        private CinematicModel CinematicModel => ModelsSystem.Instance._cinematicModel;

        private SocketBehaviour[] Sockets => _partBehaviour.Sockets;
        private bool _needBuildDoc;

        #region MonoBehaviour
        private void OnEnable()
        {
            StorageModel.TryProcessing(ConstructionDockModel._Data);

            GameUpdateModel.OnUpdate += OnUpdate;
            ConstructionDockModel.NeedBuildDocChanged += OnNeedBuildDocChanged;
            ConstructionDockModel.OnDockBuilded += OnDockBuilded;
            ShelterUpgradeModel.OnGetShowUpgradeChanged += TEMP_SetUpgradeProcessPreview;

            if(TutorialModel.IsComplete && !ConstructionDockModel.DockBuilded)
            {
                ConstructionDockModel.BuildDock();
            }

            if(ConstructionDockModel.NeedBuildDock)
            {
                OnNeedBuildDocChanged(true);
            }
            if(ConstructionDockModel.DockBuilded)
            {
                ShowDockObject();
            }

            TEMP_SetUpgradeProcessPreview(ShelterUpgradeModel.ShowUpgrade);
        }

        private void OnDisable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            ConstructionDockModel.NeedBuildDocChanged -= OnNeedBuildDocChanged;
            ConstructionDockModel.OnDockBuilded -= OnDockBuilded;
            ShelterUpgradeModel.OnGetShowUpgradeChanged -= TEMP_SetUpgradeProcessPreview;

            ShelterUpgradeModel.OnCompleteUpgrade -= OnCompleateUpgrade;
        }

        private void OnValidate()
        {
            if (_dockConstructedCinematic == null)
            {
                Debug.LogError("Dock cinematic link is missing");
            }
        }
        #endregion

        // called by GameEventListener on gameObject from  SO_event_tutorial_dockBuilded
        public void BuildDock()
        {
            ConstructionDockModel.BuildDock();
        }


        private void OnNeedBuildDocChanged(bool needBuildDoc)
        {
            _ghostPreview.SetActive(needBuildDoc);
            _needBuildDoc = needBuildDoc;
        }


        private void OnUpdate() 
        {
            if(_needBuildDoc)
            {
                foreach(var socket in Sockets)
                {
                    if(!socket.CheckOccupancy(socket.PartOffsets[0].Part))
                    {
                       return;
                    }
                }

                OnNeedBuildDocChanged(false);

                _dockConstructedCinematic.SetActive(true);
                ShelterUpgradeModel.PreCompleateUpgrade();
                ShelterUpgradeModel.OnCompleteUpgrade += OnCompleateUpgrade;
            }
        }

        private void OnCompleateUpgrade()
        {
            ShelterUpgradeModel.OnCompleteUpgrade -= OnCompleateUpgrade;
            ConstructionDockModel.NeedBuildDock = false;
        }


        private void OnDockBuilded()
        {
            ShowDockObject();
        }

        private void ShowDockObject()
        {
            foreach(var socket in Sockets)
            {
                foreach(var occupancy in socket.BusySpaces)
                {
                    
                    occupancy.Part.GetComponent<WorldObjectModel>()?.Delete();
                    Destroy(occupancy.Part.gameObject);
                }
            }
            _ghostPreview.SetActive(false);
            _dockObject.SetActive(true);
        }

        private void TEMP_SetUpgradeProcessPreview(bool show)
        {
            _upgradeProcessPreview.SetActive(show);
        }
    }
}
