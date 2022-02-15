using System;
using System.Collections.Generic;
using Game.Views;
using UnityEngine;

namespace Game.Models
{
    public class ViewsStateModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private List<ViewID> _canShowOverViews;

#pragma warning restore 0649
        #endregion
        private bool oldHUDBlockedByWindows;
        private bool oldHUDBlockedByPopups;
        private bool oldIsHUDBlocked;

        public event Action OnOpenWindowsChanged;
        public event Action OnOpenPopupsChanged;

        public event Action OnIsHUDBlocked_ByWindowsChanged;
        public event Action OnIsHUDBlocked_ByPopupsChanged;
        public event Action OnIsHUDBlocked_ByOversChanged;
        public event Action OnIsHUDBlockedChanged;

        public int OpenedOverLayersCount { get; private set; }

        public int OpenedWindowsCount { get; private set; }
        public int OpenedPopupsCount { get; private set; }

        public void SetOpenWindowsCount(int count)
        {
            OpenedWindowsCount = count;
            OnOpenWindowsChanged?.Invoke();
            UpdateIsHudBlocked();
        }

        public void SetOpenPopupsCount(int count)
        {
            OpenedPopupsCount = count;
            OnOpenPopupsChanged?.Invoke();
            UpdateIsHudBlocked();
        }

        private void UpdateIsHudBlocked()
        {
            if (oldIsHUDBlocked != IsHUDBlocked)
            {
                oldIsHUDBlocked = IsHUDBlocked;
                OnIsHUDBlockedChanged?.Invoke();
            }
        }

        public IEnumerable<ViewID> CanShowOverViews => _canShowOverViews;
        public bool WindowOpened() => OpenedWindowsCount > 0;
        public bool PopupOpened() => OpenedPopupsCount > 0;

        public bool WindowOrPopupOpened() => WindowOpened() || PopupOpened();

        public bool IsHUDBlocked_ByWindows => OpenedWindowsCount > 0;
        public bool IsHUDBlocked_ByPopups => OpenedPopupsCount > 0;
        public bool IsHUDBlocked_ByOverlayers => OpenedOverLayersCount > 0;
        public bool IsHUDBlocked => IsHUDBlocked_ByWindows || IsHUDBlocked_ByPopups;
    }
}
