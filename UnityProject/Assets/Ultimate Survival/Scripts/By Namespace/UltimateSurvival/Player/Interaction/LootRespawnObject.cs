using Core.Storage;
using Game.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public class LootRespawnObject : MonoBehaviour, IData
    {
        [Serializable]
        public class Data : DataBase
        {
            public List<long> RespawnTimes;

            public void AddRespawnTime(long time)
            {
                RespawnTimes.Add(time);
                ChangeData();
            }

            public void SetRespawnTimeAt(int pos, long time)
            {
                RespawnTimes[pos] = time;
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;

        [SerializeField] private long _intervalRespawn;
        [SerializeField] private LootObject _lootObject;

#pragma warning restore 0649
        #endregion

        public LootObject LootObject => _lootObject;
        public float IntervalRespawn => _intervalRespawn;

        public List<long> GetRespawnTimes => _data.RespawnTimes;

        public event Action OnDataInitialize;
        
        private GameTimeModel GameTimeModel => ModelsSystem.Instance._gameTimeModel;

        private Dictionary<CellModel, Coroutine> Coroutines { get; } = new Dictionary<CellModel, Coroutine>();

        public IEnumerable<IUnique> Uniques
        {
            get
            {
                yield return _data;
            }
        }

        private void OnEnable()
        {
            LootObject.OnOpen += OnOpenHandler;
            LootObject.OnClose += OnCloseHandler;
            LootObject.OnDataInitialize += OnUUIDInitializeHandler;
        }

        private void OnDisable()
        {
            LootObject.OnOpen -= OnOpenHandler;
            LootObject.OnClose -= OnCloseHandler;
            LootObject.OnDataInitialize -= OnUUIDInitializeHandler;

            StopSpawnCoroutines();
        }

        private void StopSpawnCoroutines()
        {
            foreach (var coroutine in Coroutines.Values)
            {
                StopCoroutine(coroutine);
            }

            Coroutines.Clear();
        }

        private void OnOpenHandler()
        {
            AddMissingCellTimes();

            var exepts = LootObject.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item.ItemData);
            var emptyCells = LootObject.ItemsContainer.Cells.Where(x => x.IsEmpty);

            foreach (var cell in emptyCells)
            {
                if (GameTimeModel.RealTimeNowTick >= GetLastSpawnTime(cell))
                {
                    RegenerateItem(cell, exepts);
                }
                else
                {
                    Coroutines[cell] = StartCoroutine(SpawnItem(cell));
                }
            }

            LootObject.ItemsContainer.OnChangeCell += OnChangeCellHandler;
            LootObject.ItemsContainer.OnAddCell += OnAddCellHandler;
        }

        private void OnCloseHandler()
        {
            LootObject.ItemsContainer.OnChangeCell -= OnChangeCellHandler;
            LootObject.ItemsContainer.OnAddCell -= OnAddCellHandler;

            StopSpawnCoroutines();
        }

        private void OnUUIDInitializeHandler()
        {
            LootObject.OnDataInitialize -= OnUUIDInitializeHandler;

            LootObject.StorageModel.TryProcessing(_data);
        }

        private void AddMissingCellTimes()
        {
            for (int i = GetRespawnTimes.Count; i < LootObject.ItemsContainer.CountCells; i++)
            {
                AddCellTime();
            }
        }

        private void OnAddCellHandler(CellModel cell)
        {
            AddCellTime();
            RegenerateItem(cell);
        }

        private void AddCellTime()
        {
            _data.AddRespawnTime(0);
        }

        private void OnChangeCellHandler(CellModel cell)
        {
            if (cell.IsEmpty)
            {
                UpdateRespanwTime(cell);
                Coroutines[cell] = StartCoroutine(SpawnItem(cell));
            }
        }

        private IEnumerator SpawnItem(CellModel cell)
        {
            yield return new WaitForSecondsRealtime(GameTimeModel.GetSecondsTotal(GetLastSpawnTime(cell) - GameTimeModel.RealTimeNowTick));
            RegenerateItem(cell);
            Coroutines.Remove(cell);
        }

        private void RegenerateItem(CellModel cell)
        {
            var exepts = LootObject.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item.ItemData);
            RegenerateItem(cell, exepts);
        }

        private void RegenerateItem(CellModel cellModel, IEnumerable<ItemData> excepts)
        {
            if (LootObject.CellsSettings[cellModel.Id].TryGenerateItem(LootObject.ItemsDB.ItemDatabase, out var item, excepts))
            {
                cellModel.Item = item;
                UpdateRespanwTime(cellModel);
            }
        }

        private void UpdateRespanwTime(CellModel cellModel)
        {
            _data.SetRespawnTimeAt(cellModel.Id, GameTimeModel.RealTimeNowTick + GameTimeModel.GetTicks(IntervalRespawn));
        }

        private long GetLastSpawnTime(CellModel cellModel)
        {
            return GetRespawnTimes[cellModel.Id];
        }

        public void UUIDInitialize() => OnDataInitialize?.Invoke();
    }
}
