using UnityEngine;

namespace Game.Views
{
    public struct CraftCellData
    {
        public int ItemId;
        public int CellId;
        public Sprite Icon;
        public string Name;
        public bool IsLocked;
        public bool IsCanUnlock;
        public bool IsEpic;
        public bool IsActive;
        public bool IsSelected;
    }
}
