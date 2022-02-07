using System;
using Core.Storage;
using UnityEngine;

namespace Game.Models
{
    public class PerformanceStartCheckModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
           public bool performanceChecked;

           public void SetPerformanceChecked(bool value)
           {
               performanceChecked = value;
               ChangeData();
           }
        }

        [SerializeField] Data _data = default;
        [SerializeField] private int lowFPS = 20;
        [SerializeField] private float startDelay = 1f;

        public bool PerformanceChecked
        {
            get{ return _data.performanceChecked;}
            set{ _data.SetPerformanceChecked(value);}
        }
        public int LowFPS => lowFPS;
        public float StartDelay => startDelay;

        private void OnEnable() 
        {
            ModelsSystem.Instance._storageModel.TryProcessing(this._data);
        }
    }
}
