using System;
using UnityEngine;

namespace UltimateSurvival
{
    [Serializable]
    public class ExtractionSetting
    {

        #region Data
#pragma warning disable 0649

        [SerializeField] private FPTool.ToolPurpose _toolID;
        [SerializeField] private float _extractionRate;

#pragma warning restore 0649
        #endregion

        public FPTool.ToolPurpose ToolID
        {
            get
            {
                return _toolID;
            }
        }

        public float ExtractionRate
        {
            get
            {
                return _extractionRate;
            }
        }
    }
}
