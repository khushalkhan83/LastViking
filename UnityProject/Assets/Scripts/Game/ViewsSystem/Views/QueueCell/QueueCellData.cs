using UnityEngine;

namespace Game.Views
{
    public struct QueueCellData
    {
        public int CellId;
        public int ItemId;
        public bool isBurn;
        public bool IsSelected;
        public bool isBoost;
        public float Progress;
        public Sprite ItemIcon;
        public string TimeLeft;
    }
}
