using Game.Models;
using System.Linq;
using UnityEngine;

namespace UltimateSurvival
{
    public class FPTool : FPMelee
    {
        private HintsModel HintsModel => ModelsSystem.Instance._hintsModel;
        private HotBarModel HotBarModel => ModelsSystem.Instance._hotBarModel;

        public enum ToolPurpose { CutWood, BreakRocks,Dig }


        #region Data
#pragma warning disable 0649

        [Header("Tool Settings")]

        [Tooltip("Useful for making the tools gather specific resources (eg. an axe should gather only wood, pickaxe - only stone)")]

        [SerializeField] private ExtractionSetting[] _extractionSettings;

        public ExtractionSetting[] ExtractionSettings => _extractionSettings;

#pragma warning restore 0649
        #endregion

        protected override void On_Hit()
        {
            base.On_Hit();

            var raycastData = Player.RaycastData.Value;

            if (raycastData == null
                || raycastData.GameObject == null
                || HotBarModel.EquipCell == null 
                || HotBarModel.EquipCell.Item == null 
                || HotBarModel.EquipCell.Item.ItemData == null)
                return;

            var mineable = raycastData.GameObject.GetComponent<MineableObject>();

            if (mineable)
            {
                HintsModel.CheckHintIsNeeded(mineable);

                mineable.OnToolHit(raycastData.CameraRay, raycastData.HitInfo, ExtractionSettings);

                var tool = ExtractionSettings.FirstOrDefault(x => x.ToolID == mineable.RequiredToolPurpose);
                if (tool != null)
                {
                    OnDecreaseDurability();
                }
            }
        }
    }
}
