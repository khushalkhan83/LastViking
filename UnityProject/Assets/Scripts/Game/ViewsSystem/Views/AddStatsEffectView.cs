using Core.Views;
using System;
using System.Collections.Generic;
using UltimateSurvival;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Game.Views
{
    public class AddStatsEffectView : ViewBase
    {
        public enum StatID
        {
            Health,
            Food,
            Water,
        }

        public struct StatInfo
        {
            public StatID StatID;
            public int Count;
        }

        [Serializable]
        public class StatData
        {
            #region Data
#pragma warning disable 0649

            [SerializeField] private Sprite _icon;
            [SerializeField] private Color _color;

#pragma warning restore 0649
            #endregion

            public Sprite Icon => _icon;
            public Color Color => _color;
        }

        #region Data
#pragma warning disable 0649

        [SerializeField] private StatData[] _statDatas;
        [SerializeField] private Transform _container;
        [SerializeField] private float _angleBetween;
        [SerializeField] private float _offsetXBetween;
        [SerializeField] private float _offsetXRandom;

#pragma warning restore 0649
        #endregion

        public float AngleBetween => _angleBetween;
        public float OffsetXBetween => _offsetXBetween;
        public float OffsetXRandom => _offsetXRandom;

        public Transform Container => _container;

        public void SetPosition(Vector3 position) => transform.position = position;

        public ViewsSystem ViewsSystem => ViewsSystem.Instance;

        private ItemDatabase _itemDatabase;
        public ItemDatabase ItemDatabase => _itemDatabase ?? (_itemDatabase = FindObjectOfType<ItemDatabase>());

        private StatData GetStatData(StatID statID) => _statDatas[(int)statID];

        public event Action OnEndAll;
        public ushort CountShow { get; private set; }

        public List<AddStatEffectView> AddStatEffectViews { get; } = new List<AddStatEffectView>();

        public void StartEffect(params StatInfo[] statInfos)
        {
            var count = (ushort)statInfos.Length;
            for (ushort i = 0; i < count; i++)
            {
                ShowStat(statInfos[i], count, i);
            }
        }

        public void EndEffect()
        {
            while (AddStatEffectViews.Count > 0)
            {
                AddStatEffectViews[0].EndPlay();
            }
        }

        private void ShowStat(StatInfo statInfo, ushort count, ushort index)
        {
            var statData = GetStatData(statInfo.StatID);
            var view = ViewsSystem.Show<AddStatEffectView>(ViewConfigID.AddStatEffect, Container);

            view.OnEndPlay += OnEndPlayHandler;
            view.SetOffset(GetOfset(count, index, OffsetXBetween, OffsetXRandom));
            view.SetIcon(statData.Icon);
            view.SetText($"{statInfo.Count:+#;-#;+0}");
            view.SetIconColor(statData.Color);
            view.SetTextColor(statData.Color);
            view.SetAngle(GetAngle(count, index, AngleBetween));
            view.PlayRandomly();

            AddStatEffectViews.Add(view);
        }

        private void OnEndPlayHandler(AddStatEffectView view)
        {
            --CountShow;
            view.OnEndPlay -= OnEndPlayHandler;
            ViewsSystem.Hide(view);

            AddStatEffectViews.Remove(view);

            if (CountShow == 0)
            {
                OnEndAll?.Invoke();
            }
        }

        private Vector3 GetOfset(ushort count, ushort index, float offsetXBetween, float offsetXRandom)
        {
            var all = offsetXBetween * (count - 1);
            var x = (all / 2) - (index * offsetXBetween);

            x += Random.Range(-offsetXRandom, offsetXRandom);

            return new Vector3(x, 0);
        }

        private float GetAngle(ushort count, ushort index, float angleBetween)
        {
            var all = angleBetween * (count - 1);

            return (all / 2) - (index * angleBetween);
        }
    }
}
