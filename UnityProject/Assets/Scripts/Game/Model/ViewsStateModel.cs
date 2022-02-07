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

        public event Action OnOpenWindowsChanged;
        public event Action OnOpenPopupsChanged;

        public int OpenWindowsCount {get; private set;}
        public int OpenPopupsCount {get; private set;}

        public void SetOpenWindowsCount(int count)
        {
            OpenWindowsCount = count;
            OnOpenWindowsChanged?.Invoke();
        }

        public void SetOpenPopupsCount(int count)
        {
            OpenPopupsCount = count;
            OnOpenPopupsChanged?.Invoke();
        }

        public IEnumerable<ViewID> CanShowOverViews => _canShowOverViews;
        public bool WindowOpened() => OpenWindowsCount > 0;
        public bool PopupOpened() => OpenPopupsCount > 0;

        public bool WindowOrPopupOpened() => WindowOpened() || PopupOpened();
    }
}
