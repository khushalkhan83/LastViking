using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class TombModel : MonoBehaviour, IData
    {
        [Serializable]
        public class Data : DataBase
        {
            public ObscuredULong PlayerLifeTime;
            public ObscuredUInt PlayerKilled;
            public ObscuredString PlayerName;
            public ObscuredInt PlayerImageIndex;
            [ObscuredID(typeof(ShelterModelID))]
            public ObscuredInt ShelterID;
            public ObscuredInt ShelterLevel;

            public List<SavableItem> Items;


            public void SetPlayerLifeTime(ulong playerLifeTime)
            {
                PlayerLifeTime = playerLifeTime;
                ChangeData();
            }

            public void SetPlayerKilled(uint playerKilled)
            {
                PlayerKilled = playerKilled;
                ChangeData();
            }

            public void SetPlayerName(string playerName)
            {
                PlayerName = playerName;
                ChangeData();
            }

            public void SetPlayerImageIndex(int playerImageIndex)
            {
                PlayerImageIndex = playerImageIndex;
                ChangeData();
            }

            public void SetShelterId(int shelterId)
            {
                ShelterID = shelterId;
                ChangeData();
            }

            public void SetShelterLevel(int shelterLevel)
            {
                ShelterLevel = shelterLevel;
                ChangeData();
            }

            public void SetItems(List<SavableItem> items)
            {
                Items = items;
                ChangeData();
            }

            public void RemoveItem(SavableItem item)
            {
                Items.Remove(item);
                ChangeData();
            }

            public void ClearItems()
            {
                Items.Clear();
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

#pragma warning restore 0649
        #endregion

        public List<SavableItem> Items
        {
            get => _data.Items;
            protected set => _data.SetItems(value);
        }

        public ulong PlayerLifeTime
        {
            get => _data.PlayerLifeTime;
            protected set => _data.SetPlayerLifeTime(value);
        }

        public uint PlayerKilled
        {
            get => _data.PlayerKilled;
            protected set => _data.SetPlayerKilled(value);
        }

        public string PlayerName
        {
            get => _data.PlayerName;
            protected set => _data.SetPlayerName(value);
        }

        public int PlayerImageIndex
        {
            get => _data.PlayerImageIndex;
            protected set => _data.SetPlayerImageIndex(value);
        }

        public ShelterModelID ShelterID
        {
            get => (ShelterModelID)(int)_data.ShelterID;
            protected set => _data.SetShelterId((int)value);
        }

        public int ShelterLevel
        {
            get => _data.ShelterLevel;
            protected set => _data.SetShelterLevel(value);
        }

        public void RemoveTombItem(SavableItem item) => _data.RemoveItem(item);
        public void ClearTombItems() => _data.ClearItems();


        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;

        public event Action OnDataInitialize;
        public event Action OnDestroyTomb;

        public void Initialize(IEnumerable<SavableItem> items, ulong playerLifeTime, uint playerKilled, string playerName, int playerImageIndex, ShelterModelID shelterModelID, int shelterLevel)
        {
            Items = items.OrderByDescending(x => x.HasProperty("Priority") ? x.GetProperty("Priority").Int.Current : 0).Select(x => new SavableItem(x)).ToList();
            PlayerLifeTime = playerLifeTime;
            PlayerKilled = playerKilled;
            PlayerName = playerName;
            PlayerImageIndex = playerImageIndex;
            ShelterID = shelterModelID;
            ShelterLevel = shelterLevel;
        }

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }
        public void UUIDInitialize()
        {
            StorageModel.TryProcessing(_data);

            OnDataInitialize?.Invoke();
        }

        public void DestroyTomb() => OnDestroyTomb?.Invoke();
    }
}
