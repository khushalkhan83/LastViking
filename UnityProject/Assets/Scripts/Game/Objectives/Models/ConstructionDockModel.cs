using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using UnityEngine;
using SOArchitecture;

namespace Game.Models
{
    public class ConstructionDockModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredBool DockBuilded;

            public void SetDockBuilded(bool value)
            {
                DockBuilded = value;
                ChangeData();
            }
        }

        #region Data
    #pragma warning disable 0649

        [SerializeField] Data _data;
        [SerializeField] private GameEvent _gameEventDockBuilded;

    #pragma warning restore 0649
        #endregion

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public Data _Data => _data;

        #region MonoBehaviour
        private void OnEnable() 
        {
            StorageModel.TryProcessing(_data);
        }
            
        #endregion
        public Action<bool> NeedBuildDocChanged;
        public Action OnDockBuilded;


        private bool _needBuildDock;
        
        public bool NeedBuildDock 
        {
            get{ return _needBuildDock;}
            set
            {
                _needBuildDock = value;
                NeedBuildDocChanged?.Invoke(value);
            }
        }

        public bool DockBuilded
        {
            get{ return _data.DockBuilded; }
            set{ _data.SetDockBuilded(value); }
        }

        public void BuildDock()
        {
            DockBuilded = true;
            OnDockBuilded?.Invoke();
            _gameEventDockBuilded?.Raise();
        }

    }
}
