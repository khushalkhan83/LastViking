using Core.Storage; 
using System;
using UnityEngine;

namespace Game.Models
{
    public class TutorialProgressModel : MonoBehaviour
    {
        [Serializable]
        public class TutorialProgressData : DataBase, IImmortal
        { 
            public int _lastTutorialStep;
            public bool _startSended;

            public void SetLastTutorialStep(int value)  
            { 
                _lastTutorialStep = value;
                ChangeData();
            }

            public void SetStartSended(bool value)
            {
                _startSended = value;
                ChangeData();
            }
        } 

        #region Data
#pragma warning disable 0649
        [SerializeField] private TutorialProgressData _data; 
#pragma warning restore 0649
        #endregion

        private bool datatInited = false;

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        public TutorialProgressData Data => _data; 
        public int LastTutorialStep { 
            get { return _data._lastTutorialStep; }
            set { _data.SetLastTutorialStep(value); }
        }

        public bool StartSended {
            get { return _data._startSended; }
            set { _data.SetStartSended(value); }
        }

        public event Action OnTutorialStart;
        public event Action<int> OnTutorialStep;
        public event Action OnTutorialCompleted;

        public void Init()
        {
            if(datatInited) return;

            StorageModel.TryProcessing(_data);
            datatInited = true;
        }

        public void TutorialStart()
        {
            OnTutorialStart?.Invoke();
        }

        public void TutorialStep(int step)
        {
            LastTutorialStep = step;
            OnTutorialStep?.Invoke(step);
        }

        public void TutorialCompleted()
        {
            OnTutorialCompleted?.Invoke();
        }
    }
}
