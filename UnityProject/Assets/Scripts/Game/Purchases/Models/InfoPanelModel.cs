using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class InfoPanelModel : MonoBehaviour
    {
        public Transform ViewContainer { get; private set; }

        public event Action<ItemData> OnUpdateItemInfo;
        public event Action OnHideItemInfo;

        public void SetViewContainer(Transform container) => ViewContainer = container;

        public void UpdateItemInfo(ItemData item) => OnUpdateItemInfo?.Invoke(item);
        public void HideItemInfo() => OnHideItemInfo?.Invoke();
    }
}
