using System;
using System.Collections.Generic;
using Game.Controllers;
using UnityEngine;

namespace ActivitiesLog.ViewControllerData
{
    public class ActivitiesViewData : IDataViewController
    {
        public List<ViewData> ViewDatas { get; private set; }
        public event Action OnDataChanged;
        public event Action OnFetchDataRequest;
        public event Action OnCloseRequest;

        public void SetViewDatas(List<ViewData> viewDatas)
        {
            ViewDatas = viewDatas;
            OnDataChanged?.Invoke();
        }

        public void Fetch()
        {
            OnFetchDataRequest?.Invoke();
        }

        public void CloseViewRequest()
        {
            OnCloseRequest?.Invoke();
        }
    }
    public class ViewData
    {
        public readonly Sprite icon;
        public readonly string description;
        public readonly Color textBackgroundColor;
        public readonly Color iconBackgroundColor;
        public readonly float iconScale;

        public ViewData(Sprite icon, string description, Color textBackgroundColor, Color iconBackgroundColor, float iconScale = 1)
        {
            this.icon = icon;
            this.description = description;
            this.textBackgroundColor = textBackgroundColor;
            this.iconScale = iconScale;
            this.iconBackgroundColor = iconBackgroundColor;
        }
    }
}