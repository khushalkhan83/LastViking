using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class BloodEffectModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _healthMin;

#pragma warning restore 0649
        #endregion

        public float HealthMin => _healthMin;
    }
}
