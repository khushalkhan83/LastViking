using System;
using System.IO;
using Core.Storage;
using Game.Storage;
using UnityEngine;

namespace Game.Models
{
    public class IntroLoadingModel: MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private Data _data;
        #pragma warning restore 0649
        #endregion

        private IStorage Storage { get; } 
#if UNITY_EDITOR
            = new StorageEditor();
#else
            = new StorageMobile();
#endif

        public Data _Data => _data;

        [Serializable]
        public class Data: DataBase
        {
            public bool introPlayed = false;
        }

        public void InitData()
        {
            var path = Path.Combine((Storage as StorageFiles).RootPath, _data.UUID);
            if (File.Exists(path))
            {
                JsonUtility.FromJsonOverwrite(File.ReadAllText(path), _Data);
            }
        }
    }
}