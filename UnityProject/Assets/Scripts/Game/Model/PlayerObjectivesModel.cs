using Core.Storage;
using CustomeEditorTools;
using Game.Objectives;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Game.Objectives.Stacks;
using System.Linq;

namespace Game.Models
{
    public class PlayerObjectivesModel : MonoBehaviour
    {
        [Serializable]
        public struct ObjectiveDataInfo
        {
            public byte Id;
            public bool IsHasValue;
            public ushort Value;
        }

        [Serializable]
        public class Data : DataBase, IImmortal
        {
            public ObjectiveDataInfo[] Pool;
            public byte TierId;
            public byte ConfigurationId;

            public long NextTierTime;
            public bool ObjectivesInitedFirstTime;

            public bool IsFree;

            [SerializeField] private bool _bonusTimeAvaliable = true;
            
            public bool BonusTimeAvaliable
            {
                get { return _bonusTimeAvaliable;}
                set { _bonusTimeAvaliable = value; ChangeData();}
            }

            public void SetToPool(byte id, ObjectiveDataInfo value)
            {
                if (Pool[id].Value != value.Value || Pool[id].IsHasValue != value.IsHasValue)
                {
                    Pool[id] = value;
                    ChangeData();
                }
            }

            public void SetIsFree(bool isFree)
            {
                if (IsFree != isFree)
                {
                    IsFree = isFree;
                    ChangeData();
                }
            }

            public void SetTierId(byte id)
            {
                if (TierId != id)
                {
                    TierId = id;
                    ChangeData();
                }
            }

            public void SetConfigurationId(byte id)
            {
                if (ConfigurationId != id)
                {
                    ConfigurationId = id;
                    ChangeData();
                }
            }

            public void SetNextTierTime(long time)
            {
                NextTierTime = time;
                ChangeData();
            }

            public void SetObjectivesInitedFirstTime(bool objectivesInitedFirstTime)
            {
                ObjectivesInitedFirstTime = objectivesInitedFirstTime;
                ChangeData();
            }
        }

        [Serializable]
        public class ConfigurationData
        {
            [SerializeField] private ObjectiveID[] _objectiveIDs;

            public ObjectiveID[] ObjectiveIDs => _objectiveIDs;
        }

        [Serializable]
        public class TierData
        {
            [SerializeField] private ConfigurationData[] _configuration;

            public ConfigurationData[] Configurations => _configuration;

        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private Data _data;
        [SerializeField] private StorageModel _storageModel;
        [SerializeField] private GameTimeModel _gameTimeModel;
        [SerializeField] private RealTimeModel _realTimeModel;
        [SerializeField] private TierData[] _tiers;
        [Header("Time options")]
        [SerializeField] private float _firstTimeDelay;
        [SerializeField] private float _resetTime;
        [SerializeField] private float _bonusTime;

        [Header("Debug time options")]
        [SerializeField] private float _debugTimeReset;
        [SerializeField] private float _debugBonusTime;

#pragma warning restore 0649
        #endregion
        
        public static bool UseDebugTime {get; private set;} = false; // static for testing in editor (core restart)
        public float TimeReset => UseDebugTime ? _debugTimeReset : _resetTime;
        public float BonusTime => UseDebugTime ? _debugBonusTime : _bonusTime;

        public float FirstTimeDelay => _firstTimeDelay;
        public ObjectiveDataInfo[] Pool => _data.Pool;
        public StorageModel StorageModel => _storageModel;
        public GameTimeModel GameTimeModel => _gameTimeModel;
        public RealTimeModel RealTimeModel => _realTimeModel;
        public TierData[] Tiers => _tiers;

        public bool PoolIsEmpty {get => Pool.Where(x => x.IsHasValue).Count() == 0;}
        public bool HasTasks {get => !PoolIsEmpty;}

        public bool BonusTimeAvaliable
        {
            get => _data.BonusTimeAvaliable;
            private set => _data.BonusTimeAvaliable = value;
        }

        public byte TierId
        {
            get => _data.TierId;
            private set => _data.SetTierId(value);
        }

        public bool IsFree
        {
            get => _data.IsFree;
            private set => _data.SetIsFree(value);
        }

        public byte ConfigurationId
        {
            get => _data.ConfigurationId;
            private set => _data.SetConfigurationId(value);
        }

        public long NextTierTime
        {
            get => _data.NextTierTime;
            private set => _data.SetNextTierTime(value);
        }

        public bool ObjectivesInitedFirstTime
        {
            get => _data.ObjectivesInitedFirstTime;
            set => _data.ObjectivesInitedFirstTime = value;
        }

        public delegate void ActionPool(byte id);

        public event Action OnEndTime;
        public event Action OnPreEndTime;
        public event ActionPool OnPreEndTask;
        public event ActionPool OnPostEndTask;
        public event ActionPool OnCreate;
        public event ActionPool OnChange; /* Check logic */
        public event Action OnResetPlayerObjectives;
        public event Action<ObjectiveModel,int> OnRerollObjective;
        public event Action OnResetPlayerObjectivesOnStart;
        public event Action<byte, ObjectiveModel> OnAddToPool_Temp;
        public event Action OnUpdatePool;

        public float ReminingTime { get; set; }
        public bool IsFirst { get; private set; }

        public bool NoTimeLeft {get => ReminingTime <= 0;}

        public int CountObjectives => Tiers[TierId].Configurations[ConfigurationId].ObjectiveIDs.Length;

        private bool _inited;
        public void Init()
        {
            if(_inited) return;

            _inited = true;

            if (!StorageModel.TryProcessing(_data))
            {
                IsFirst = true;
            }
        }

        [Button]
        public void ResetPlayerObjectives() => OnResetPlayerObjectives?.Invoke();

        public void AddToPool_Temp(byte index, ObjectiveModel model) => OnAddToPool_Temp?.Invoke(index,model);
        public void UpdatePool() => OnUpdatePool?.Invoke();

        public void SetUseDebugTime(bool value) => UseDebugTime = value;

        public void GetFree()
        {
            IsFree = false;
        }

        public void SetToPool(byte id, ushort? objectiveId)
        {
            var data = NulableUshortToObjectiveDataInfo(id, objectiveId);
            _data.SetToPool(id, data);

            if (data.Equals(_data.Pool[id]))
            {
                OnChange?.Invoke(id);
            }
        }

        private ObjectiveDataInfo NulableUshortToObjectiveDataInfo(byte id, ushort? value) => new ObjectiveDataInfo()
        {
            Id = id,
            IsHasValue = value.HasValue,
            Value = value ?? default
        };

        public void End(byte id)
        {
            OnPreEndTask?.Invoke(id);
            SetToPool(id, null);
            OnPostEndTask?.Invoke(id);
        }

        public void Clear(byte id)
        {
            SetToPool(id,null);
        }

        public void EndAll()
        {
            for (byte id = 0; id < Pool.Length; id++)
            {
                if (Pool[id].IsHasValue)
                {
                    End(id);
                }
            }
        }
        
        // used by FixOldObjectivesController to clear pull on startup, to avoid invoking events (useed instead of EndAll)
        public void ClearPool()
        {
            for (byte id = 0; id < Pool.Length; id++)
            {
                if (Pool[id].IsHasValue)
                {
                    Clear(id);
                }
            }
        }

        public void Create(byte id, ushort objectiveId)
        {
            SetToPool(id, objectiveId);
            OnCreate?.Invoke(id);
        }

        public void ProcessingTime(float deltaTime)
        {
            ReminingTime -= deltaTime;

            if (NoTimeLeft)
            {
                NextTier();
            }
        }

        public void SyncRemainingTime(float remainingTime)
        {
            ReminingTime = remainingTime;
        }
        public void SetNextTierTime(long nextTime)
        {
            NextTierTime = nextTime;
        }

        public void NextTier()
        {
            OnPreEndTime?.Invoke();
            GenerateNextObjectives();
            EndAll();
            OnEndTime?.Invoke();
        }

        public void RerollObjective(ObjectiveModel objectiveModel, int index)
        {
            OnRerollObjective?.Invoke(objectiveModel, index);
        }

        public void ResetPlayerObjectivesOnStart()
        {
            OnResetPlayerObjectivesOnStart?.Invoke();
        }
        private void GenerateNextObjectives()
        {
            if (IsFirst) //TODO: add generated support
            {
                IsFirst = false;
            }
            else
            {
                NextTierId();
            }

            GenerationConfigurationId();
        }

        private void NextTierId()
        {
            if (TierId < Tiers.Length - 1)
            {
                ++TierId;
            }
            else
            {
                TierId = (byte)Random.Range(0, Tiers.Length);
            }
        }

        private void GenerationConfigurationId() => ConfigurationId = (byte)Random.Range(0, Tiers[TierId].Configurations.Length);

        public void SetBonusTimeAvaliable(bool value) => BonusTimeAvaliable = value;
    }
}
