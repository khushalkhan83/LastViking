using CodeStage.AntiCheat.ObscuredTypes;
using Core.Storage;
using Game.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;
using static Game.Models.CraftViewModel;

namespace Game.Models
{
    public class CraftModel : MonoBehaviour
    {
        [Serializable]
        public class Data : DataBase
        {
            public override SaveTime TimeSave => SaveTime.Deffered;

            public ObscuredInt UpgradeLevel;
            public ObscuredFloat BoostRemainingTime;
            public List<ItemData> Queue;
            public List<CraftInfo> Craft;
            public List<SavableItem> CraftCompleted;

            public void SetUpgradeLevel(int level)
            {
                UpgradeLevel = level;
                ChangeData();
            }

            public void SetBoostRemainingTime(float time)
            {
                BoostRemainingTime = time;
                ChangeData();
            }

            public void AddToQueue(ItemData data)
            {
                Queue.Add(data);
                ChangeData();
            }

            public void RemoveFromQueueAt(int index)
            {
                Queue.RemoveAt(index);
                ChangeData();
            }

            public void AddToCraft(CraftInfo data)
            {
                Craft.Add(data);
                ChangeData();
            }

            public void SetToCraftAt(int index, CraftInfo data)
            {
                Craft[index] = data;
                ChangeData();
            }

            public void RemoveFromCraftAt(int index)
            {
                Craft.RemoveAt(index);
                ChangeData();
            }

            public void CraftProcess() => ChangeData();

            public void AddToCraftCompleted(SavableItem item)
            {
                CraftCompleted.Add(item);
                ChangeData();
            }

            public void RemoveFromCraftCompletedAt(int index)
            {
                CraftCompleted.RemoveAt(index);
                ChangeData();
            }
        }

        [Serializable]
        public class ItemCost
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private string _name;
            [SerializeField] private int _count;

#pragma warning restore 0649
            #endregion

            public string Name { get { return _name; } }
            public int Count { get { return _count; } }
        }

        [Serializable]
        public class DataImmortal : DataBase, IImmortal
        {
            public List<ObscuredInt> Unlocked;

            public void AddToUnlocked(int item)
            {
                Unlocked.Add(item);
                ChangeData();
            }
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredInt _countCraftCellsStart;
        [SerializeField] private ObscuredInt _countCraftCellsMax;
        [SerializeField] private ObscuredInt _countQueueCellsStart;
        [SerializeField] private ObscuredInt _countQueueCellsMax;
        [SerializeField] private ObscuredInt _levelUpgradeMax;
        [SerializeField] private ObscuredFloat _boostTime;
        [SerializeField] private ObscuredFloat _boostMultiplier;

        [SerializeField] private Data _data;
        [SerializeField] private DataImmortal _dataImmortal;

#pragma warning restore 0649
        #endregion

        #region Dependencies

        private StorageModel StorageModel => ModelsSystem.Instance._storageModel;
            
        #endregion

        private bool _dataInited = false;
        internal void Init()
        {
            if(_dataInited)
            {
                return;
            }
            _dataInited = true;

            StorageModel.TryProcessing(_data);
            StorageModel.TryProcessing(_dataImmortal);
        }
        #region MonoBehaviour

        private void OnEnable()
        {
            Init();
        }

        #endregion

        public AudioSystem AudioSystem => AudioSystem.Instance;
        public int CountQueueCellsStart => _countQueueCellsStart;
        public int CountQueueCellsMax => _countQueueCellsMax;
        public int CountCraftCellsStart => _countCraftCellsStart;
        public int CountCraftCellsMax => _countCraftCellsMax;
        public int LevelUpgradeMax => _levelUpgradeMax;
        public float BoostTime => _boostTime;
        public float BoostMultiplier => _boostMultiplier;

        public IList<ItemData> Queue => _data.Queue;
        public IList<CraftInfo> Craft => _data.Craft;
        public IList<ObscuredInt> Unlocked => _dataImmortal.Unlocked;
        public Data _Data => _data;
        public DataImmortal _DataImmortal => _dataImmortal;
        private ActionsLogModel ActionsLogModel => ModelsSystem.Instance._actionsLogModel;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;
        private StatisticsModel StatisticsModel => ModelsSystem.Instance._statisticsModel;

        protected IList<SavableItem> CraftCompleted => _data.CraftCompleted;
        public IEnumerable<SavableItem> CachedCraftedItems => _data.CraftCompleted;
        public int CountCachedCraftedItems => CraftCompleted.Count;

        public float BoostRemainingTime
        {
            get => _data.BoostRemainingTime;
            private set => _data.SetBoostRemainingTime(value);
        }

        public int UpgradeLevel
        {
            get => _data.UpgradeLevel;
            private set => _data.SetUpgradeLevel(value);
        }

        //

        public int CountAllQueueCells => IsMaxLevelUpgrade ? CountQueueCellsMax : CountQueueCellsStart;
        public int CountCraftCellsAtLevel => CountCraftCellsStart + UpgradeLevel;
        public int CountQueueCells => CountAllQueueCells - CountCraftCellsAtLevel;
        public int CountCells => Craft.Count + Queue.Count;
        public bool IsMaxLevelUpgrade => UpgradeLevel == LevelUpgradeMax;
        public bool IsBoostNow => BoostRemainingTime > 0;

        [Serializable]
        public class CraftInfo
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private ItemData _itemData;
            [SerializeField] private ObscuredFloat _allTime;
            [SerializeField] private ObscuredFloat _remainingTime;

#pragma warning restore 0649
            #endregion

            public bool IsEnd => RemainingTime <= 0;

            public ItemData ItemData
            {
                get => _itemData;
                set => _itemData = value;
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

        //

        public event Action OnUpgrade;
        public event Action OnChangeCount;
        public event Action OnStartBoost;
        public event Action OnUpdateBoostRemainingTime;
        public event Action OnEndBoost;
        public event Action<int> OnUnlockedItem;
        public event Action<int> OnCraftItemStarted;
        public event Action<int> OnCraftedItem;

        //

        public void PushCraftCompleted(SavableItem item)
        {
            _Data.AddToCraftCompleted(item);
        }

        public SavableItem PopCraftCompleted()
        {
            var item = CraftCompleted[CraftCompleted.Count - 1];
            _Data.RemoveFromCraftCompletedAt(CraftCompleted.Count - 1);
            return item;
        }

        public void CraftProcess(float deltaTime)
        {
            foreach (var item in Craft)
            {
                item.RemainingTime -= GetDeltaTime(deltaTime);
            }

            if (Craft.Count > 0)
                _Data.CraftProcess();

            //processing crafted items
            for (int i = 0; i < Craft.Count; i++)
            {
                if (Craft[i].IsEnd)
                {
                    OnCraftedItem?.Invoke(Craft[i].ItemData.Id);

                    SendCraftActionMessage(Craft[i].ItemData);

                    if (Queue.Count > 0)
                    {
                        _Data.SetToCraftAt(i, GetCraftInfo(Queue[0]));
                        RemoveItemFromQueue(0);
                    }
                    else
                    {
                        RemoveItemFromCraft(i);
                        --i;
                    }
                }
            }

            //shift items from queue to craft
            for (int i = Craft.Count; i < CountCraftCellsAtLevel && Queue.Count > 0; i++)
            {
                _Data.AddToCraft(GetCraftInfo(Queue[0]));
                RemoveItemFromQueue(0);
            }

            if (BoostRemainingTime > 0)
            {
                BoostRemainingTime -= Time.deltaTime;

                OnUpdateBoostRemainingTime?.Invoke();

                if (BoostRemainingTime <= 0)
                {
                    BoostRemainingTime = 0;
                    OnEndBoost?.Invoke();
                }
            }
        }

        private void SendCraftActionMessage(ItemData item)
        {
            int value = item.Recipe.CraftCount;

            var itemData = ItemsDB.GetItem(item.Name);
            ActionsLogModel.SendMessage(new MessageInventoryCraftedData(value, itemData));

            var message = new ActionInventoryChanged(item, value, ActionInventoryChanged.ChangeType.Crafted);
            ActionsLogModel.SendMessage(message);
            AudioSystem.PlayOnce(AudioID.CraftEnd);
            StatisticsModel.CraftItem();
        }

        private float GetDeltaTime(float deltaTime)
        {
            if (IsBoostNow)
            {
                return deltaTime * BoostMultiplier;
            }

            return deltaTime;
        }

        public bool TryAddItem(ItemData item)
        {
            if (Craft.Count < CountCraftCellsAtLevel)
            {
                _Data.AddToCraft(GetCraftInfo(item));
                OnChangeCount?.Invoke();
                OnCraftItemStarted?.Invoke(item.Id);
                return true;
            }
            else if (CountCells < CountAllQueueCells)
            {
                _Data.AddToQueue(item);
                OnChangeCount?.Invoke();
                OnCraftItemStarted?.Invoke(item.Id);
                return true;
            }

            return false;
        }

        public bool IsUnlocked(int itemId) => Unlocked.Contains(itemId);

        public void UnlockItem(int itemId)
        {
            _DataImmortal.AddToUnlocked(itemId);
            OnUnlockedItem?.Invoke(itemId);
        }

        public void RemoveItemFromCraft(int index)
        {
            _Data.RemoveFromCraftAt(index);

            OnChangeCount?.Invoke();
        }

        public void RemoveItemFromQueue(int index)
        {
            _data.RemoveFromQueueAt(index);

            OnChangeCount?.Invoke();
        }

        public void Upgrade()
        {
            ++UpgradeLevel;

            OnUpgrade?.Invoke();
        }

        public void Boost()
        {
            BoostRemainingTime = BoostTime;

            OnStartBoost?.Invoke();
        }

        private CraftInfo GetCraftInfo(ItemData item)
        {
            return new CraftInfo()
            {
                ItemData = item,
                RemainingTime = item.Recipe.Duration,
                AllTime = item.Recipe.Duration
            };
        }

        public IEnumerable<ItemData> GetItemsByCategory(CategoryID categoryID)
        {
            var categoryPredicate = GetCategoryPredicate(categoryID);
            var craftableItems = ItemsDB.ItemDatabase.Categories.SelectMany(category => category.Items).Where(categoryPredicate);
            return craftableItems;
        }

        private Func<ItemData, bool> GetCategoryPredicate(CategoryID categoryID)
        {
            if (categoryID != CategoryID.None)
            {
                return item => item.IsCraftable && item.Recipe.CategoryID == categoryID;
            }

            return item => item.IsCraftable;
        }
    }
}