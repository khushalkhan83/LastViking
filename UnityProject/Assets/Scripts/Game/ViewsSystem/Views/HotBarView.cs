using Core.Views;
using Game.Views.Cell;
using UnityEngine;

namespace Game.Views
{
    public class HotBarView : ViewBase
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private HotBarCellView[] _cells;

#pragma warning restore 0649
        #endregion

        public HotBarCellView[] Cells => _cells;
    }
}
