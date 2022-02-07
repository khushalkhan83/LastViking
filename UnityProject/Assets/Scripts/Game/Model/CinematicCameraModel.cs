using System;
using Cinemachine;
using UnityEngine;

namespace Game.Models
{
    public class CinematicCameraModel : MonoBehaviour
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private CinemachineBrain _cinemachineBrain;
        [SerializeField] private CinemachineVirtualCamera _firstPersonVirtualCamera;
        [SerializeField] private Camera _worldCamera;


#pragma warning restore 0649
        #endregion
        public CinemachineBrain CinemachineBrain => _cinemachineBrain;
        public CinemachineVirtualCamera FirstPersonVirtualCamera => _firstPersonVirtualCamera;
        public Camera WorldCamera => _worldCamera;

        public float PlayerCameraFOV {get; private set;}
        public event Action OnActiveChanged;
        public bool CameraActive {get; private set;}
        
        public void Init()
        {
            PlayerCameraFOV = WorldCamera.fieldOfView;
        }
        public void SetCameraActive(bool active)
        {
            CameraActive = active;
            OnActiveChanged?.Invoke();
        }
    }
}
