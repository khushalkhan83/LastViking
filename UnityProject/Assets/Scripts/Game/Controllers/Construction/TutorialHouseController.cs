using EasyBuildSystem.Runtimes.Internal.Part;
using UnityEngine;
using EasyBuildSystem.Runtimes.Internal.Socket;
using Game.Models;
using static Game.Models.TutorialHouseModel;

namespace Game.Controllers
{
    public class TutorialHouseController : MonoBehaviour
    {
         #region Data
    #pragma warning disable 0649
        [SerializeField] TutorialHousePart housePart;
        [SerializeField] PartBehaviour _partBehavior;
        [SerializeField] GameObject _ghostObject;

    #pragma warning restore 0649
        #endregion

        private bool _needBuildPart;

        private SocketBehaviour[] Sockets => _partBehavior.Sockets;
        private TutorialHouseModel TutorialHouseModel => ModelsSystem.Instance._tutorialHouseModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private GameUpdateModel GameUpdateModel => ModelsSystem.Instance._gameUpdateModel;
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
        private PlayerScenesModel PlayerScenesModel => ModelsSystem.Instance._playerScenesModel;

        #region MonoBehaviour
        private void OnEnable()
        {
            StorageModel.TryProcessing(TutorialHouseModel._Data);

            PlayerScenesModel.OnEnvironmentLoaded += OnEnvironmentLoaded;
        }

        private void OnDisable()
        {
            PlayerScenesModel.OnEnvironmentLoaded -= OnEnvironmentLoaded;
            GameUpdateModel.OnUpdate -= OnUpdate;
            TutorialHouseModel.NeedBuildPartChanged -= OnNeedBuildPartChanged;
            TutorialHouseModel.OnPartBuilded -= OnPartBuilded;
        }
            
        #endregion

        private void OnEnvironmentLoaded()
        {
            // depend on tutorial step instead of serialization
            TutorialHouseModel.SetNeedBuildPart(housePart, (!TutorialModel.IsComplete) && GetHouseTutorialStep(housePart) == TutorialModel.Step);

            GameUpdateModel.OnUpdate += OnUpdate;
            TutorialHouseModel.NeedBuildPartChanged += OnNeedBuildPartChanged;
            TutorialHouseModel.OnPartBuilded += OnPartBuilded;
            if(TutorialHouseModel.GetNeedBuildPart(housePart))
            {
                OnNeedBuildPartChanged(housePart, true);
            }
            if(TutorialHouseModel.IsBuilded(housePart))
            {
                OnNeedBuildPartChanged(housePart, false);
            }
        }


        private void OnNeedBuildPartChanged(TutorialHousePart part, bool needBuildPart)
        {
            if(housePart == part)
            {
                _needBuildPart = needBuildPart;
                UpdateView();
            }
        }

        private void UpdateView()
        {
            _ghostObject.SetActive(_needBuildPart);
            _partBehavior.gameObject.SetActive(_needBuildPart);
        }

        private void OnPartBuilded(TutorialHousePart part) 
        {
            if(housePart == part)
                OnNeedBuildPartChanged(part, false);
        }

        private void OnUpdate() 
        {
            if(_needBuildPart)
            {
                foreach(var socket in Sockets)
                {
                    if(!socket.CheckOccupancy(socket.PartOffsets[0].Part))
                    {
                       return;
                    }
                }

                TutorialHouseModel.SetNeedBuildPart(housePart, false);
                TutorialHouseModel.BuildPart(housePart);
            }
        }


        private int GetHouseTutorialStep(TutorialHousePart part)
        {
            switch(part)
            {
                case TutorialHousePart.Foundation:
                    return  TutorialModel.BuildFoundationStep;
                case TutorialHousePart.Walls:
                    return  TutorialModel.BuildWallsStep;
                case TutorialHousePart.Roof:
                    return  TutorialModel.BuildRoofStep;
                case TutorialHousePart.Door:
                    return  TutorialModel.BuildDoorStep;
                default:
                    return 0;
            }
        }
    }
}
