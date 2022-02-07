using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.Views
{
    public class InventoryHotBarView : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649
        [SerializeField] private CellView[] _hotBarCells;
#pragma warning restore 0649
        #endregion

        public CellView[] HotBarCells => _hotBarCells;
    }
}
