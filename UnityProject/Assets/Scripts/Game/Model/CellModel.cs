using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class CellModel
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private int _id;
        [SerializeField] private SavableItem _item;

#pragma warning restore 0649
        #endregion

        public event OnChangeCell OnChange;

        public int Id => _id;

        public bool IsHasItem => Item != null && Item.Count > 0 && Item.Id > 0;
        public bool IsEmpty => !IsHasItem;

        public SavableItem Item
        {
            get
            {
                return _item;
            }
            set
            {
                if (IsHasItem)
                {
                    _item.PropertyChanged.RemoveListener(PropertyChangedHandler);
                    _item.StackChanged.RemoveListener(CountChangedHandler);
                }

                _item = value;

                if (IsHasItem)
                {
                    _item.PropertyChanged.AddListener(PropertyChangedHandler);
                    _item.StackChanged.AddListener(CountChangedHandler);
                }

                Change();
            }
        }

        private void CountChangedHandler()
        {
            Change();
        }

        private void PropertyChangedHandler(ItemProperty.Value obj)
        {
            Change();
        }

        private void Change()
        {
            OnChange?.Invoke(this);
        }

        public CellModel() { }

        public CellModel(int id, SavableItem savableItem = null)
        {
            _id = id;
            Item = savableItem;
        }

        public void Load()
        {
            if (IsHasItem)
            {
                _item.PropertyChanged.AddListener(PropertyChangedHandler);
                _item.StackChanged.AddListener(CountChangedHandler);
            }
        }
    }
}
