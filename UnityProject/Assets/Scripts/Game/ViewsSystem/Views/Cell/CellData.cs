using UnityEngine;
using UltimateSurvival;

namespace Game.Views
{
    public struct CellData
    {
        public int Id;
        public int ContainerId;
        public Sprite Icon;
        public int? Count;
        public float? Durability;
        public float? Sips;
        public float? ProgressConsumable;
        public float? ProgressRepairing;
        public bool IsDisable;
        public ItemRarity ItemRarity;
        public bool IsComponent;
    }
}
