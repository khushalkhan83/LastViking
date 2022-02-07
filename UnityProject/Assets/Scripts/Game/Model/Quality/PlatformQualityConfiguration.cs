using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Models
{
    [Serializable]
    public class PlatformQualityConfiguration
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private QualityID _defaultQuaityID;
        [SerializeField] private QualityConfig[] _qualityConfigs;

#pragma warning restore 0649
        #endregion

        public QualityID DefaultQuaityID => _defaultQuaityID;
        public IEnumerable<QualityConfig> QualityConfigs => _qualityConfigs;

        public QualityConfig GetConfig(QualityID quaityID) => _qualityConfigs[(int)quaityID - 1];
    }
}
