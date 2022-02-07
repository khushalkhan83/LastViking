using UnityEngine;
using UltimateSurvival;

namespace Game.Views
{
    public struct ResourceCellData
    {
        public int ItemId;
        public int CellId;
        public Sprite Icon;
        public string Message;
        public bool IsActive;
        public bool IsSelected;
        public ItemRarity ItemRarity;
        public bool IsComponent;

        public static ResourceCellData Empty => new ResourceCellData()
        {
            ItemId = -1,
            CellId = -1
        };
    }
}
