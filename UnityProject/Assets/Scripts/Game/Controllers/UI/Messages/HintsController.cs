using System;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using static UltimateSurvival.FPTool;
using Extensions;

namespace Game.Controllers
{
    public class HintsController : IHintsController, IController
    {
        [Inject] public HintsModel HintsModel { get; private set; }
        [Inject] public MineableObjectsModel MineableObjectsModel { get; private set; }
        [Inject] public HotBarModel HotBarModel { get; private set; }
        void IController.Enable() 
        {
            HintsModel.OnTryShowMinableToolHint += OnTryShowMinableToolHint;
            HintsModel.OnCheckHintIsNeeded += OnCheckHintIsNeeded;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            HintsModel.OnTryShowMinableToolHint -= OnTryShowMinableToolHint;
            HintsModel.OnCheckHintIsNeeded -= OnCheckHintIsNeeded;
        }

        private void OnCheckHintIsNeeded(MineableObject mineable)
        {
            if(mineable == null) return;

            if ((mineable.RequiredToolPurpose == ToolPurpose.CutWood && HotBarModel.EquipCell.Item.HasProperty("MiningWood")) ||
                (mineable.RequiredToolPurpose == ToolPurpose.BreakRocks && HotBarModel.EquipCell.Item.HasProperty("MiningStone")) ||
                HotBarModel.EquipCell.Item.Name == "tool_hook")
            {
                SetShowHint(mineable,true);
            }
            else
            {
                SetShowHint(mineable,false);
            }
        }

        private void OnTryShowMinableToolHint(GameObject gameObject)
        {
            var mineable = gameObject.CheckNull()?.GetComponent<MineableObject>();

            if (mineable)
            {
                SetShowHint(mineable,false);
            }
        }

        private void SetShowHint(MineableObject mineable, bool value)
        {
            switch (mineable.GetComponent<MinebleFractureObject>()?.RequiredToolPurpose)
            {
                case ToolPurpose.CutWood:
                    MineableObjectsModel.HasWoodTutorialShown = value;
                    break;
                case ToolPurpose.BreakRocks:
                    MineableObjectsModel.HasStoneTutorialShown = value;
                    break;
            }
        }
    }
}
