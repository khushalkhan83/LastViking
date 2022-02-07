using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    public class ObjectivesNotificationModel : MonoBehaviour
    {
        public event Action<ushort> OnShow;
        public event Action<ushort> OnHide;

        public class Data
        {
            public float Progress;
            public bool IsShowNow;
        }

        public Dictionary<ushort, Data> Datas { get; } = new Dictionary<ushort, Data>();

        public void ClearDatas() => Datas.Clear();

        public void ChangeProgress(ushort id, float progress)
        {
            var data = Datas[id];
            if (IsCanShow(data.IsShowNow, progress, data.Progress))
            {
                Show(id);
            }
            data.Progress = progress;
        }

        public void Show(ushort id)
        {
            if (Datas.ContainsKey(id))
            {
                Datas[id].IsShowNow = true;
                OnShow?.Invoke(id);
            }
        }

        public void Hide(ushort id)
        {
            if (Datas.ContainsKey(id))
            {
                Datas[id].IsShowNow = false;
                OnHide?.Invoke(id);
            }
        }

        public void Close(ushort id)
        {
            if (Datas.ContainsKey(id))
            {
                var data = Datas[id];
                data.IsShowNow = false;
                data.Progress = 0;
            }
        }

        public void Create(ushort id)
        {
            if (!Datas.ContainsKey(id))
            {
                Datas[id] = new Data();
            }
        }

        public bool IsCanShow(bool isShow, float progressCurrent, float progressLast) =>
            !isShow
            &&
            (
                progressCurrent > 0 && progressLast <= 0
                || progressCurrent >= 0.5f && progressLast < 0.5f
                || (progressCurrent >= 1 && progressLast < 1)
            );
    }
}
