using System;
using System.Collections.Generic;
using Core.Storage;
using Game.Views;
using UnityEngine;

namespace Game.Models
{
    public class ConstructionInfoModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public bool isConstructionInfoShown;

            public void SetIsConstructionInfoShown(bool value)
            {
                isConstructionInfoShown = value;
                ChangeData();
            }
        }

        public event Action ConstructionInfoShown;

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] float _showConstructionInfoDelay;
        [SerializeField] private List<ViewID> _canShowOverViews;

#pragma warning restore 0649
        #endregion


        public float ShowConstructionInfoDelay => _showConstructionInfoDelay;
        public List<ViewID> CanShowOverViews => _canShowOverViews;


        public bool IsConstructionInfoShown
        {
            get { return _data.isConstructionInfoShown; }
            set { _data.SetIsConstructionInfoShown(value); }
        }

        public void ShowConstructionInfo()
        {
            IsConstructionInfoShown = true;
            ConstructionInfoShown?.Invoke();
        }

        #region MonoBehaviour
        private void OnEnable()
        {
            ModelsSystem.Instance._storageModel.TryProcessing(_data);
        }
            
        #endregion

    }
}
