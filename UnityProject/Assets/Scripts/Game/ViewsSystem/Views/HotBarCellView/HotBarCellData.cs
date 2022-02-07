using UnityEngine;

namespace Game.Views.Cell
{
    public struct HotBarCellData
    {
        public int Id;
        public Sprite Icon;
        public int? Count;
        public float? Durability;
        public float? Sips;
        public float? ProgressConsumable;
        public float? ProgressRepairing;
        public bool IsDisable;
        public bool IsVisibleBorder;
    }
}
