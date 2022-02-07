using CodeStage.AntiCheat.ObscuredTypes;
using UnityEngine;

namespace Game.Models
{
    public class RotateBuildablePreviewModel : MonoBehaviour
    {
        #region Data
#pragma warning disable 0649

        [SerializeField] private ObscuredFloat _rotateAngle;
        [SerializeField] private ObscuredFloat _rotateAngleStart;

#pragma warning restore 0649
        #endregion

        public float RotateAngle => _rotateAngle;
        public float RotateAngleStart => _rotateAngleStart;

        public float RotationAngleCurrent { get; private set; }

        public void Rotate() => RotationAngleCurrent = (RotationAngleCurrent + RotateAngle) % 360;

        public void Reset() => RotationAngleCurrent = RotateAngleStart;
    }
}
