
using CodeStage.AntiCheat.ObscuredTypes;
using Core;
using Core.Storage;
using Game.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Purchasing;

namespace Game.Purchases
{
    public class PurchaseComplitedModel : MonoBehaviour
    {
        [Serializable]
        protected class Data: DataBase, IImmortal
        {
            #region Data
#pragma warning disable 0649

            public ObscuredBool _anyPurchaseComplited;

#pragma warning restore 0649
            #endregion

            public void SetAnyPurchaseComplited(bool value) {
                _anyPurchaseComplited = value;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        public bool AnyPurchaseComplited 
        {
            get { return _data._anyPurchaseComplited; }
            set { _data.SetAnyPurchaseComplited(value); }
        }

        private void OnEnable()
        {
            ModelsSystem.Instance._storageModel.TryProcessing(_data);
        }

    }
}
