using Core.Storage;
using Game.Providers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class SheltersModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public ShelterModelID ShelterActive;
            public List<ShelterModelID> Buyed;
            public ulong TimeLastGetCoins;

            public override SaveTime TimeSave => SaveTime.Instantly;

            public void SetShelterActive(ShelterModelID shelterModelID)
            {
                ShelterActive = shelterModelID;
                ChangeData();
            }

            public void SetTimeLastGetCoins(ulong time)
            {
                TimeLastGetCoins = time;
                ChangeData();
            }

            public void AddBuyed(ShelterModelID id)
            {
                Buyed.Add(id);
                ChangeData();
            }

            public void ClearBuyed()
            {
                Buyed.Clear();
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private ShelterModelsProvider _shelterModels;

#pragma warning restore 0649
        #endregion

        public IList<ShelterModelID> Buyed => _data.Buyed;

        public ShelterModelID ShelterActive
        {
            get => _data.ShelterActive;
            protected set => _data.SetShelterActive(value);
        }

        public ulong TimeLastGetCoins
        {
            get => _data.TimeLastGetCoins;
            protected set => _data.SetTimeLastGetCoins(value);
        }

        public bool IsBuyed(ShelterModelID shelterID) => Buyed.Contains(shelterID);

        //

        public ShelterModel ShelterModel { get; private set; }

        private ShelterModelsProvider ShelterModelsProvider => _shelterModels;
        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;

        public class ShelterModelEvents
        {
            public Action OnBuy { get; }
            public Action OnActivate { get; }
            public Action OnUpgrade { get; }
            public Action OnDeath { get; }

            public ShelterModelEvents(Action onBuy, Action onActivate, Action onUpgrade, Action onDeath)
            {
                OnBuy = onBuy;
                OnActivate = onActivate;
                OnUpgrade = onUpgrade;
                OnDeath = onDeath;
            }
        }

        public event Action<ShelterModel> OnActivate;
        public event Action<ShelterModel> OnBuy;
        public event Action<ShelterModel> OnUpgrade;
        public event Action<ShelterModel> OnDeath;
        public event Action<ShelterModel> OnProtectedFromSkeletons;

        public void ProtectedFromSkeletons() => OnProtectedFromSkeletons?.Invoke(ShelterModelsProvider[ShelterActive]);

        protected Hashtable ShelterModelsActions { get; } = new Hashtable();

        private void OnEnable()
        {
            if (StorageModel.TryProcessing(_data))
            {
                if (ShelterActive != ShelterModelID.None)
                {
                    ShelterModel = ShelterModelsProvider[ShelterActive];
                }
            }

            foreach (var shelterModel in ShelterModelsProvider)
            {
                var events = new ShelterModelEvents
                (
                    () => OnBuyHandler(shelterModel.ShelterID)
                    , () => OnActivateHandler(shelterModel.ShelterID)
                    , () => OnUpgradeHandler(shelterModel.ShelterID)
                    , () => OnDeathHandler(shelterModel.ShelterID)
                );
                ShelterModelsActions.Add(shelterModel, events);

                shelterModel.OnBuy += events.OnBuy;
                shelterModel.OnActivate += events.OnActivate;
                shelterModel.OnUpgrade += events.OnUpgrade;
                shelterModel.OnDeath += events.OnDeath;
            }
        }

        private void OnDisable()
        {
            foreach (var shelterModel in ShelterModelsProvider)
            {
                var events = ShelterModelsActions[shelterModel] as ShelterModelEvents;
                ShelterModelsActions.Remove(shelterModel);

                shelterModel.OnBuy -= events.OnBuy;
                shelterModel.OnActivate -= events.OnActivate;
                shelterModel.OnUpgrade -= events.OnUpgrade;
                shelterModel.OnDeath -= events.OnDeath;
            }
        }

        private void OnDeathHandler(ShelterModelID shelterID)
        {
            OnDeath?.Invoke(ShelterModelsProvider[shelterID]);
        }

        private void OnUpgradeHandler(ShelterModelID shelterID)
        {
            OnUpgrade?.Invoke(ShelterModelsProvider[shelterID]);
        }

        private void OnBuyHandler(ShelterModelID shelterID)
        {
            _data.AddBuyed(shelterID);
            OnBuy?.Invoke(ShelterModelsProvider[shelterID]);
        }

        private void OnActivateHandler(ShelterModelID shelterID)
        {
            ShelterActive = shelterID;
            OnActivate?.Invoke(ShelterModelsProvider[shelterID]);
        }

        public void SetShelter(ShelterModel shelterModel)
        {
            ShelterModel = shelterModel;
        }

        public void SetTimeLastGetCoins(ulong ticks)
        {
            TimeLastGetCoins = ticks;
        }
    }
}
