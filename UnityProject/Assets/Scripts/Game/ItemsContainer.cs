using System;
using System.Collections.Generic;
using System.Linq;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public delegate void OnChangeCell(CellModel cell);
    public delegate void OnAddCell(CellModel cell);
    public delegate void OnRemoveCell(CellModel cell);
    public delegate void OnAddItems(int itemId, int count);
    public delegate void OnPreAddItems(SavableItem item, int count);

    [Serializable]
    public class ItemsContainer
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private List<CellModel> _cells;

#pragma warning restore 0649
        #endregion

        public event OnChangeCell OnChangeCell;
        public event OnAddCell OnAddCell;
        public event OnAddItems OnAddItems;
        public event OnPreAddItems OnPreAddItems;
        public event Action OnDataLoadedFromDataBase;

        public IEnumerable<CellModel> Cells => _cells;

        public int CountCells => _cells.Count;
        private ItemsDB ItemsDB => ModelsSystem.Instance._itemsDB;

        public void OnLoad()
        {
            foreach (var cell in _cells)
            {
                cell.OnChange += OnChangeCellHandler;

                if (cell.IsHasItem)
                {
                    if(ItemsDB.TryGetItem(cell.Item.Id, out ItemData item))
                    {
                        cell.Item.ItemData = item;
                    }
                    else
                    {
                        cell.Item = null;
                    }
                }

                cell.Load();
            }

            OnDataLoadedFromDataBase?.Invoke();
        }

        private void OnChangeCellHandler(CellModel cell) => OnChangeCell?.Invoke(cell);

        public int GetItemsCount(int itemId) => Cells.Where(cell => cell.IsHasItem && cell.Item.Id == itemId).Sum(cell => cell.Item.Count);

        public bool IsHasItemInCell(int cellId, int itemId)
        {
            var cell = GetCell(cellId);
            return cell.IsHasItem && cell.Item.Id == itemId;
        }

        public bool IsHasItemInCell(int cellId) => GetCell(cellId).IsHasItem;

        public bool IsEmptyCell(int cellId) => !GetCell(cellId).IsHasItem;

        public bool IsHasItem(int itemId) => _cells.Select(cell => cell.Item).Any(item => item != null && item.Id == itemId);

        public bool IsHasItems(int itemId, int count) => _cells.Select(cell => cell.Item).Where(item => item != null && item.Id == itemId).Sum(x => x.Count) >= count;

        public int GetEmptyCellsCount() => Cells.Count(cell => cell.IsEmpty);

        public int AddItems(int itemId, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("You cannot add a negative number of elements");
            }

            return AdjustItemsCount(itemId, count);
        }

        public int AddItemsData(SavableItem item, int count)
        {
            if (item.Count < 0)
            {
                throw new ArgumentOutOfRangeException("You cannot add a negative number of elements");
            }

            return AdjustItemsCountData(item, count);
        }

        public bool AddAllItems(out IEnumerable<SavableItem> leftItems, params SavableItem[] items) => AddAllItems(items, out leftItems);

        public bool AddAllItems(IEnumerable<SavableItem> items, out IEnumerable<SavableItem> leftItems)
        {
            var result = new List<SavableItem>();
            var isFill = false;
            var left = 0;
            var emptyCell = (CellModel)null;

            foreach (var item in items)
            {
                if (ItemsDB.GetItem(item.Id).StackSize > 1)
                {
                    left = AddItems(item.Id, item.Count);
                    if (left > 0)
                    {
                        item.Count = left;
                        result.Add(item);
                    }
                }
                else
                {
                    if (isFill)
                    {
                        result.Add(item);
                    }
                    else
                    {
                        emptyCell = Cells.Where(x => x.IsEmpty).FirstOrDefault();
                        if (emptyCell == null)
                        {
                            result.Add(item);
                            isFill = true;
                        }
                        else
                        {
                            SetItem(emptyCell.Id, item);
                        }
                    }
                }
            }

            leftItems = result;
            return result.Count == 0;
        }

        public int RemoveItems(int itemId, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("You cannot remove a negative number of elements");
            }

            return -AdjustItemsCount(itemId, -count);
        }

        public int RemoveItemsFromCell(int cellId, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("You cannot remove a negative number of elements");
            }

            if (_cells[cellId].IsHasItem)
            {
                return -AdjustCountInCell(cellId, GetCell(cellId).Item.Id, -count);
            }

            return count;
        }

        public void ClearCell(int cellId) => GetCell(cellId).Item = null;

        /// <summary>
        /// Change amount of items in cells
        /// </summary>
        /// <param name="itemId">id of item</param>
        /// <param name="count">amount of items</param>
        /// <returns>count is left</returns>
        public int AdjustItemsCount(int itemId, int count)
        {
            if (count == 0)
            {
                return 0;
            }

            //adjust count in has items
            for (int cellId = 0; cellId < _cells.Count; cellId++)
            {
                if (IsHasItemInCell(cellId, itemId))
                {
                    count = AdjustCountInCell(cellId, itemId, count);

                    if (count == 0)
                    {
                        return 0;
                    }
                }
            }

            //Added items to empty cells
            if (count > 0)
            {
                for (int cellId = 0; cellId < _cells.Count; cellId++)
                {
                    if (IsEmptyCell(cellId))
                    {
                        count = AdjustCountInCell(cellId, itemId, count);

                        if (count == 0)
                        {
                            return 0;
                        }
                    }
                }
            }

            return count;
        }
        public int AdjustItemsCountData(SavableItem item, int count)
        {
            if (count == 0)
            {
                return 0;
            }

            //adjust count in has items
            for (int cellId = 0; cellId < _cells.Count; cellId++)
            {
                if (IsHasItemInCell(cellId, item.Id))
                {
                    count = AdjustCountInCellData(cellId, item, count);

                    if (count == 0)
                    {
                        return 0;
                    }
                }
            }

            //Added items to empty cells
            if (count > 0)
            {
                for (int cellId = 0; cellId < _cells.Count; cellId++)
                {
                    if (IsEmptyCell(cellId))
                    {
                        count = AdjustCountInCellData(cellId, item, count);

                        if (count == 0)
                        {
                            return 0;
                        }
                    }
                }
            }

            return count;
        }
        /// <summary>
        /// adjust count of items in cell
        /// </summary>
        /// <param name="cellId">id of cell</param>
        /// <param name="count">count of items which need change in cells</param>
        /// <returns>count is left</returns>
        public int AdjustCountInCell(int cellId, int itemId, int count)
        {
            var left = count;

            if (IsEmptyCell(cellId))
            {
                if (count > 0)
                {
                    left = SetCell(cellId, itemId, left);
                }
            }
            else
            {
                left = GetItem(cellId).AdjustCount(left);

                if (GetItem(cellId).Count == 0)
                {
                    RemoveItemsFromCell(cellId);
                }
            }

            if (count - left > 0)
            {
                OnPreAddItems?.Invoke(GetItem(cellId), count - left);
                OnAddItems?.Invoke(itemId, count - left);
            }

            return left;
        }
        public int AdjustCountInCellData(int cellId, SavableItem item, int count)
        {
            var result = count;

            if (IsEmptyCell(cellId))
            {
                if (result < 0)
                {
                    return result;
                }

                result = SetCellData(cellId, item, result);

                if (count - result > 0)
                {
                    OnPreAddItems?.Invoke(item, count - result);
                    OnAddItems?.Invoke(item.Id, count - result);
                }

                return result;
            }

            result = GetItem(cellId).AdjustCount(result);

            if (GetItem(cellId).Count == 0)
            {
                RemoveItemsFromCell(cellId);
            }

            if (count - result > 0)
            {
                OnPreAddItems?.Invoke(item, count - result);
                OnAddItems?.Invoke(item.Id, count - result);
            }

            return result;
        }


        public SavableItem SplitItems(int cellId, int count)
        {
            var cell = GetCell(cellId);
            if (cell.IsEmpty)
            {
                throw new Exception();
            }

            var left = cell.Item.AdjustCount(-count);

            if (cell.Item.Count == 0)
            {
                RemoveItemsFromCell(cellId);
                count += left;
            }

            OnChangeCell?.Invoke(cell);

            return new SavableItem(cell.Item)
            {
                Count = count
            };
        }

        public void AddCells(int count)
        {
            _cells.Capacity = _cells.Count + count;

            for (int i = 0; i < count; i++)
            {
                AddCell();
            }
        }

        public void AddCell() => AddCell(null);

        public void AddCell(SavableItem savableItem)
        {
            var cell = new CellModel(CountCells, savableItem);
            cell.OnChange += OnChangeCellHandler;

            _cells.Add(cell);
            OnAddCell?.Invoke(cell);
        }

        public void RemoveLastCells(int count)
        {
            if (count < 0)
            {
                throw new ArgumentException();
            }

            for (int i = count; i > 0; i--)
            {
                _cells.RemoveAt(_cells.Count - 1);
            }
        }

        private void RemoveItemsFromCell(int cellId) => GetCell(cellId).Item = null;

        public CellModel GetCell(int cellId) => _cells[cellId];

        private SavableItem GetItem(int cellId) => GetCell(cellId).Item;

        private void SetItem(int cellId, SavableItem item) => GetCell(cellId).Item = item;

        public int SetCell(int cellId, int itemId, int count)
        {
            var item = new SavableItem(ItemsDB.GetItem(itemId), 0)
            {
                Count = 0
            };

            var leftCount = item.AdjustCount(count);

            SetItem(cellId, item);

            return leftCount;
        }
        public int SetCellData(int cellId, SavableItem Item, int count)
        {
            var item = new SavableItem(Item)
            {
                Count = 0
            };

            var leftCount = item.AdjustCount(count);

            SetItem(cellId, item);

            return leftCount;
        }

        public void RemoveAllItems()
        {
            for (int cellId = 0; cellId < CountCells; cellId++)
            {
                ClearCell(cellId);
            }
        }

        public CellModel GetEmptyCell() => Cells.FirstOrDefault(x => x.IsEmpty);

        public void AutoStackItems() {
            for (int i = 0; i < CountCells; i++)
            {
                if (IsEmptyCell(i)) continue;

                var stackItem = GetItem(i);
                int needCount = stackItem.ItemData.StackSize - stackItem.Count;

                if (needCount == 0) continue;

                for (int j = i + 1; j < CountCells; j++) 
                {
                    if (IsEmptyCell(j)) continue;

                    var addItem = GetItem(j);
                    if (addItem.Id == stackItem.Id) {
                        int addCount = Mathf.Min(needCount, addItem.Count);
                        AdjustCountInCell(i, stackItem.Id, addCount);
                        AdjustCountInCell(j, stackItem.Id, -addCount);
                        needCount = stackItem.ItemData.StackSize - stackItem.Count;

                        if (needCount == 0) 
                            break;
                    }
                }
                
            }
        }

        public bool CanAddItem(ItemData itemData, int count, out int left)
        {
            int itemID = itemData.Id;
            left = count;
            foreach(var cell in _cells)
            {
                if(cell.IsEmpty)
                {
                    left -= itemData.StackSize;
                }
                else
                {
                    if(cell.Item.Id == itemID && cell.Item.Count < itemData.StackSize)
                    {
                        int maxToAd = itemData.StackSize - cell.Item.Count;
                        left -= maxToAd;
                    }
                }

                if(left <= 0)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
