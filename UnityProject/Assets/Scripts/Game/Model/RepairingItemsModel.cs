using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using Game.Purchases;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class RepairingItemsModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public List<RepairInfo> RepairingItems;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private RepairData[] _repairData;

#pragma warning restore 0649
        #endregion
        
        [Serializable]
        public class RepairData
        {
            [SerializeField] private ObscuredFloat _durabilityPartRestores;
            [SerializeField] private ObscuredFloat _resourcesPartRequires;
            [ObscuredID(typeof(PurchaseID))]
            [SerializeField] private ObscuredInt _repairPurchaseLow;
            [ObscuredID(typeof(PurchaseID))]
            [SerializeField] private ObscuredInt _repairPurchaseMid;
            [ObscuredID(typeof(PurchaseID))]
            [SerializeField] private ObscuredInt _repairPurchaseHigh;
            [SerializeField] private ObscuredBool _onlyGoldRepair;

            public float PartRestoredDuratility => _durabilityPartRestores;
            public float PartRequiresResources => _resourcesPartRequires;
            public PurchaseID RepairPurchaseLow => (PurchaseID) (int)_repairPurchaseLow;
            public PurchaseID RepairPurchaseMid => (PurchaseID)(int)_repairPurchaseMid;
            public PurchaseID RepairPurchaseHigh => (PurchaseID)(int)_repairPurchaseHigh;
            public bool OnlyGoldRepair => _onlyGoldRepair;
        }

        [Serializable]
        public class RepairInfo
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private SavableItem _item;
            [SerializeField] private ObscuredFloat _allTime;
            [SerializeField] private ObscuredFloat _remainingTime;

#pragma warning restore 0649
            #endregion

            public bool IsEnd => RemainingTime <= 0;

            public SavableItem Item
            {
                get => _item;
                set => _item = value;
            }

            public float AllTime
            {
                get => _allTime;
                set => _allTime = value;
            }

            public float RemainingTime
            {
                get => _remainingTime;
                set => _remainingTime = value;
            }
        }

        public Data _Data => _data;

        public event Action OnChangeCount;
        public event Action OnUpdateRepairing;
        public event Action<SavableItem> OnRepairedItem;

        public IList<RepairInfo> Repairing => _data.RepairingItems;

        public float PartRestoredDuratility(int index) => _repairData[index].PartRestoredDuratility;
        public float PartRequiresResources(int index) => _repairData[index].PartRequiresResources;
        public PurchaseID RepairPurchaseLow(int index) => _repairData[index].RepairPurchaseLow;
        public PurchaseID RepairPurchaseMid(int index) => _repairData[index].RepairPurchaseMid;
        public PurchaseID RepairPurchaseHigh(int index) => _repairData[index].RepairPurchaseHigh;
        public bool OnlyGoldRepair(int index) => _repairData[index].OnlyGoldRepair;

        public void RepairProcess(float deltaTime)
        {
            if (Repairing.Count > 0)
            {
                OnUpdateRepairing?.Invoke();
            }

            foreach (var item in Repairing)
            {
                item.RemainingTime -= deltaTime;
            }

            for (int i = 0; i < Repairing.Count; i++)
            {
                if (Repairing[i].IsEnd)
                {
                    RepairItem(Repairing[i].Item);
                    OnChangeCount?.Invoke();
                    Repairing.RemoveAt(i);
                    --i;
                }
            }
        }

        public void AddItem(SavableItem item)
        {
            Repairing.Add(GetRepairInfo(item));
            OnChangeCount?.Invoke();
        }

        public void RepairItemInstantly(SavableItem item) => RepairItem(item);

        private void RepairItem(SavableItem item) => OnRepairedItem?.Invoke(item);

        public bool TryFindRepairItemInfo(SavableItem item, out RepairInfo info)
        {
            foreach (var rep in Repairing)
            {
                if (rep.Item == item)
                {
                    info = rep;
                    return true;
                }
            }

            info = null;
            return false;
        }

        private RepairInfo GetRepairInfo(SavableItem item)
        {
            float time = item.GetProperty("Repair time").Float.Default;
            return new RepairInfo()
            {
                Item = item,
                RemainingTime = time,
                AllTime = time,
            };
        }
    }
}
