using Game.Views;
using System;
using UltimateSurvival;
using UnityEngine;

namespace Game.Models
{
    public class CollectCellDataModel : MonoBehaviour
    {
        public event Func<CellModel, int, CellData> OnCollectCellData;
        public event Func<SavableItem, int, int, CellData> OnCollectConsumeCellData;

        public CellData CollectCellData(CellModel cell, int containerId) => OnCollectCellData.Invoke(cell, containerId);
        public CellData CollectConsumeCellData(SavableItem item, int containerId, int cellId) => OnCollectConsumeCellData.Invoke(item, containerId, cellId);
    }
}
