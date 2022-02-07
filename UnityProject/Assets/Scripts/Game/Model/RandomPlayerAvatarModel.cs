using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Models
{
    public class RandomPlayerAvatarModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObscuredInt Index;

            public void SetIndex(int index)
            {
                Index = index;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private Sprite[] _sprites;
        [SerializeField] private StorageModel _storageModel;

#pragma warning restore 0649
        #endregion

        public StorageModel StorageModel => _storageModel;
        public Sprite[] Sprites => _sprites;
        public Sprite PlayerAvatar => Sprites[Index];

        public int Index
        {
            get
            {
                return _data.Index;
            }
            protected set
            {
                _data.SetIndex(value);
            }
        }

        public void GenerateNextAvatarIndex()
        {
            var rnd = Random.Range(0, _sprites.Length - 1);
            Index = (Index + rnd) % _sprites.Length;
        }

        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }
    }
}
