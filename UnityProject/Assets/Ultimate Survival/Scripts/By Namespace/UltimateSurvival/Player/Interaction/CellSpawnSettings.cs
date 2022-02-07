using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace UltimateSurvival
{
    
    [Serializable]
    public class CellSpawnSettings
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private GenerateID _generatorType;

        //[SerializeField] private string[] _items;
        [SerializeField]
        private ItemSettings[] _itemSettings;
        [SerializeField] private string _itemName;
        [SerializeField] private string _categoryName;
        [SerializeField] private string[] _categoryNames;

        [SerializeField] private CountID _countType = CountID.One;

        [SerializeField] private int _count;

        [SerializeField] private int _countMin;
        [SerializeField] private int _countMax;

        [Range(0.0f, 1.0f)]
        [SerializeField] private float _countPercent;

#pragma warning restore 0649
        #endregion

        public void Function()
        {
            //    _itemSettings = new ItemSettings[_items.Length];
            //    for (int i = 0; i < _items.Length; i++)
            //    {
            //        _itemSettings[i] = new ItemSettings();
            //        _itemSettings[i].item = _items[i];
            //    }
        }

        public enum CountID
        {
            None = -1,
            One = 0,
            Count = 1,
            RandomFromRange = 2,
            PercentOfMaxStack = 3,
        }

        public enum GenerateID
        {
            EmptyItem = 0,
            RandomFromAll = 1,
            RandomFromCategory = 2,
            RandomFromItems = 3,
            ConcreteItem = 4,
            RandomFromCategories = 5,
        }

        [Serializable]
        public class ItemSettings
        {
            public string item;
            public CountID countId = CountID.None;
            public int count;

            public int countMin;
            public int countMax;

            [Range(0.0f, 1.0f)]
            public float countPercent;
        }

        public SavableItem GenerateItem(ItemDatabase itemDatabase, IEnumerable<ItemData> excepts = null)
        {
            var item = GetItem(itemDatabase, out ItemSettings settings, excepts);

            if (item != null)
            {
                int count = settings.countId == CountID.None ? GetCount(item) : GetCount(item, settings);
                item.Count = Mathf.Min(item.ItemData.StackSize, count);
            }

            return item;
        }

        public bool TryGenerateItem(ItemDatabase itemDatabase, out SavableItem item, IEnumerable<ItemData> excepts = null)
        {
            item = GenerateItem(itemDatabase, excepts);

            return item != null;
        }

        public SavableItem GetItem(ItemDatabase itemDatabase, out ItemSettings _settings, IEnumerable<ItemData> excepts = null)
        {
            var item = (ItemData)null;
            ItemSettings settings = new ItemSettings();

            switch (_generatorType)
            {
                case GenerateID.RandomFromAll:
                    item = GetRandomFromAll(itemDatabase, excepts);
                    break;
                case GenerateID.RandomFromCategories:
                    item = GetRandomFromCategories(itemDatabase, excepts);
                    break;
                case GenerateID.RandomFromCategory:
                    item = GetRandomFromCategory(itemDatabase, excepts);
                    break;
                case GenerateID.ConcreteItem:
                    item = GetItemFromName(itemDatabase, excepts);
                    break;
                case GenerateID.RandomFromItems:
                    item = GetItemFromNames(itemDatabase, out settings, excepts);
                    break;
            }

            if (item != null)
            {
                _settings = settings;
                return new SavableItem(item);
            }

            _settings = settings;
            return null;
        }

        private ItemData GetRandomFromCategories(ItemDatabase itemDatabase, IEnumerable<ItemData> excepts = null)
        {
            return GetItem(itemDatabase.Categories.Where(x => _categoryName.Contains(x.Name)).SelectMany(x => x.Items), excepts);
        }

        private ItemData GetItemFromNames(ItemDatabase itemDatabase, out ItemSettings _settings, IEnumerable<ItemData> excepts = null)
        {
            var names = _itemSettings.Select(x => x.item) as IEnumerable<string>; //_items as IEnumerable<string>;
            ItemSettings settings = new ItemSettings();

            if (excepts != null)
            {
                names = names.Except(excepts.Select(x => x.Name));
            }

            var count = names.Count();
            if (count == 0)
            {
                settings.countId = CountID.One;
                _settings = settings;
                return null;
            }

            //var name = names.Skip(Random.Range(0, count)).First();
            //var item = _itemSettings

            settings = _itemSettings.Skip(Random.Range(0, count)).First();

            //var index = Random.Range(0, count);
            string name = settings.item;// _itemSettings[index].item;// names.ElementAt(index);
            _settings = settings;// _itemSettings[index];

            return GetItem(itemDatabase, name);
        }

        public ItemData GetItemFromName(ItemDatabase itemDatabase, IEnumerable<ItemData> excepts = null)
        {
            if (excepts != null && excepts.FirstOrDefault(x => x.Name == _itemName) != null)
            {
                return null;
            }

            return GetItem(itemDatabase, _itemName);
        }

        public ItemData GetRandomFromCategory(ItemDatabase itemDatabase, IEnumerable<ItemData> excepts = null)
        {
            return GetItem(itemDatabase.Categories.SkipWhile(x => x.Name != _categoryName).First().Items, excepts);
        }

        public ItemData GetRandomFromAll(ItemDatabase itemDatabase, IEnumerable<ItemData> excepts = null)
        {
            return GetItem(itemDatabase.Categories.SelectMany(x => x.Items), excepts);
        }

        private static ItemData GetItem(IEnumerable<ItemData> items, IEnumerable<ItemData> excepts)
        {
            if (excepts != null)
            {
                items = items.Except(excepts);
            }

            return GetItem(items);
        }

        private static ItemData GetItem(IEnumerable<ItemData> items)
        {
            var count = items.Count();

            if (count == 0)
            {
                return null;
            }

            return items.Skip(Random.Range(0, count)).First();
        }

        private ItemData GetItem(ItemDatabase itemDatabase, string name)
        {
            ItemData item;
            if (itemDatabase.FindItemByName(name, out item))
            {
                return item;
            }

            return null;
        }

        public int GetCount(SavableItem item, ItemSettings settings)
        {
            switch (settings.countId)
            {
                case CountID.Count:
                    return settings.count;// _count;
                case CountID.RandomFromRange:
                    return Random.Range(settings.countMin, settings.countMax + 1);
                case CountID.PercentOfMaxStack:
                    return Math.Max(1, (int)(item.ItemData.StackSize * settings.countPercent));
            }

            return 1;
        }

        public int GetCount(SavableItem item)
        {
            switch (_countType)
            {
                case CountID.Count:
                    return _count;
                case CountID.RandomFromRange:
                    return Random.Range(_countMin, _countMax + 1);
                case CountID.PercentOfMaxStack:
                    return Math.Max(1, (int)(item.ItemData.StackSize * _countPercent));
            }

            return 1;
        }
    }
}