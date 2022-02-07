using System;
using System.Collections.Generic;
using System.Linq;
using Core.Storage;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Models
{
    public class MedallionsModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [Serializable]
        public class Data : DataBase
        {
           [SerializeField] private List<CollectableData> collectables = new List<CollectableData>();

            public void SetCollected(string id)
            {
                var target = collectables.Find(x => x.id == id);

                if(target == null)
                {
                    collectables.Add(new CollectableData(id,true));
                }
                else
                {
                    target.collected = true;
                }
                ChangeData();
            }

            public bool IsCollected(string id)
            {
                var target = collectables.Find(x => x.id == id);

                if(target == null) return false;

                return target.collected;
            }

            public void Clear()
            {
                collectables.Clear();
                ChangeData();
            }

            public int Collected() => collectables.Where(x => x.collected == true).Count();
        }

        [Serializable]
        public class CollectableData
        {
            public CollectableData(string id, bool collected)
            {
                this.id = id;
                this.collected = collected;
            }
            public string id;
            public bool collected;
        }

        [SerializeField] private Data _data;
        [SerializeField] private int _totalCount = 1;
        [SerializeField] private Sprite _icon;

        #pragma warning restore 0649
        #endregion

        public int Total => _totalCount;
        public int Collected => _data.Collected();
        public Sprite Icon => _icon;

        #region MonoBehaviour

        private void OnEnable()
        {
            ModelsSystem.Instance._storageModel.TryProcessing(_data);    
        }
            
        #endregion

        public event Action OnCollect;

        public bool IsCollected(string id)
        {
            return _data.IsCollected(id);
        }

        public void SetCollected(string id)
        {
            _data.SetCollected(id);
            OnCollect?.Invoke();
        }

        #region Development

        [Button] void Clear() => _data.Clear();
            
        #endregion
    }
}
