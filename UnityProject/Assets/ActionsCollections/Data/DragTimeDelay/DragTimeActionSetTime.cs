using System.Collections.Generic;
using ActionsCollections;
using Game.Models;
using UltimateSurvival;
using UltimateSurvival.Debugging;
using UnityEngine;

namespace DebugActions
{
    public class DragTimeActionSetTime : ActionBase
    {
        private InventoryDragAndDropModel InventoryDragAndDropModel => ModelsSystem.Instance._inventoryDragAndDropModel;

        private string _operationName;
        private float _time;

        public DragTimeActionSetTime(string name, float time)
        {
            _operationName = name;
            _time = time;
           
        }
        public override string OperationName => _operationName;


        public override void DoAction()
        {
           InventoryDragAndDropModel.SelecCellTime = _time;
        }

    }
}