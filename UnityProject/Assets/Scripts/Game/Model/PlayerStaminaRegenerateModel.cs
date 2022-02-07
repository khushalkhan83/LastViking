using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class PlayerStaminaRegenerateModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _regeneratePerSecond;

#pragma warning restore 0649
        #endregion

        public float RegeneratePerSecond => _regeneratePerSecond;

        public float GetRegeneration(float time) => RegeneratePerSecond * time;
    }
}
