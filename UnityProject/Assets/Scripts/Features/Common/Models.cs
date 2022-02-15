
using Core.Storage;
using UnityEngine;
using Game.Models;

namespace Reactive
{
    public abstract class ModelBase<T> : MonoBehaviour where T : DataBase
    {
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        [SerializeField] private T dataBase;

        protected T DataBase => dataBase;
        public bool IsDataBaseInited { get; private set; }

        // called at PreInitState
        internal void InitData()
        {
            if (IsDataBaseInited)
            {
                return;
            }
            IsDataBaseInited = true;

            if (dataBase == null) return;
            StorageModel.TryProcessing(dataBase);
        }

        #region MonoBehaviour
        private void OnEnable()
        {
            InitData();
            OnDataBaseInited();
        }
        #endregion

        protected virtual void OnDataBaseInited() { }

    }
}