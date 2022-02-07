using Core.Views;
using Game.Views.Cell;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Views
{
    public class BuildingHotBarView : ViewBase
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] List<BuildingCellView> _cells;
        [SerializeField] GameObject _buildingOptionCellPref;
#pragma warning restore 0649
        #endregion

        public GameObject BuildingOptionCellPref => _buildingOptionCellPref;
        public List<BuildingCellView> Cells => _cells;
    }
}
