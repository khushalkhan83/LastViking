
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public abstract class InitableModel<T> : MonoBehaviour where T : DataBase
    {
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        protected abstract T DataBase { get; }
        public bool Inited { get; private set; }

        // called at PreInitState
        internal void Init()
        {
            if (Inited)
            {
                return;
            }
            Inited = true;

            if (DataBase == null) return;
            StorageModel.TryProcessing(DataBase);
        }

        #region MonoBehaviour
        private void OnEnable()
        {
            Init();
            OnInited();
        }
        #endregion

        protected virtual void OnInited() { }

    }
}