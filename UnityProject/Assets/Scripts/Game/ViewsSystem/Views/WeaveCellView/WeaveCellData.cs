using UnityEngine;

namespace Game.Views
{
    public struct WeaveCellData
    {
        public int Id;
        public int ContainerId;
        public Sprite Icon;
        public int? Count;
        public float? Durability;
        public float? Sips;
        public float? ProcessAmount;
        public float? ProgressConsumable;
        public float? ProgressRepairing;
        public bool IsBoost;
        public bool IsDisable;
    }
}
