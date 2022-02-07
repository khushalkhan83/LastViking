using System;
using System.Collections.Generic;
using Core.Storage;
using EasyBuildSystem.Runtimes.Internal.Part;
using EasyBuildSystem.Runtimes.Internal.Socket;
using UnityEngine;

namespace Game.Models
{
    public class TutorialBuildingModel : MonoBehaviour
    {
        [Serializable]
        private class Data : DataBase
        {
            [SerializeField] private List<uint> _tutorialModeConstructions;

            public List<uint> TutorialModeConstructionIds
            {
                get { return _tutorialModeConstructions; }
                set { _tutorialModeConstructions = value; ChangeData(); }
            }
        }

        #region Data
        #pragma warning disable 0649
        [SerializeField] private Data _data;
        [SerializeField] float tutorialPriceModifier = default;
        #pragma warning restore 0649
        #endregion

        #region Dependencies
        private TutorialModel TutorialModel => ModelsSystem.Instance._tutorialModel;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        #endregion

        public List<uint> TutorialModeConstructionIds
        {
            get => _data.TutorialModeConstructionIds;
            private set => _data.TutorialModeConstructionIds = value;
        }

        #region MonoBehaviour

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }
        #endregion

        public bool TutorialStage => !TutorialModel.IsComplete;
        

        public Func<SocketBehaviour, bool> OnIsPlacementAllowed;
        public Func<PartBehaviour, bool> OnCanPartSnap;
        public float TutorialPriceModifier => tutorialPriceModifier;


        public void RegisterTutorialModeConstruction(uint id)
        {
            if(TutorialModeConstructionIds.Contains(id)) return;
            
            var temp = new List<uint>(TutorialModeConstructionIds);
            temp.Add(id);

            TutorialModeConstructionIds = temp;
        }
    }
}
