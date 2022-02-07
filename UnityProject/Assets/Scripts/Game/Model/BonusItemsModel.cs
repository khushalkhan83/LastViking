using System;
using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using Extensions;
using UnityEngine;

namespace Game.Models
{
    public class BonusItemsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredInt ReceivedBonusesCount;

            public void ReceiveBonus()
            {
                ReceivedBonusesCount++;
                ChangeData();
            }
        }
        
        #region Data
        #pragma warning disable 0649
        [SerializeField] public List<string> _bonusItems;
        [SerializeField] private Data _data;
#pragma warning restore 0649
        #endregion

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        #region MonoBehaviour
        private bool _inited;
        private void OnEnable()
        {
            StorageModel.TryProcessing(_data);
        }
        #endregion

        public int ReceivedBonusesCount => _data.ReceivedBonusesCount;

        public string GetBonusItem()
        {
            var itemIndex = ReceivedBonusesCount;
            if(_bonusItems.IndexOutOfRange(itemIndex))
            {
                itemIndex = _bonusItems.Count - 1;
            }
            
            return _bonusItems[itemIndex];
        }

        public void ReceiveBonus()
        {
            _data.ReceiveBonus();
        }
    }
}
