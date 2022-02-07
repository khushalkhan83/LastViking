using Cinemachine;
using UnityEngine;

namespace Game.ThirdPerson.Camera
{
    public class CameraController : MonoBehaviour
    {
    #pragma warning disable 0649
        [SerializeField] private CinemachineFreeLook freeLook;
    #pragma warning restore 0649

        private void Update()
        {
            freeLook.m_XAxis.m_InputAxisValue = PlayerInput.Instance.CameraInput.x;
            freeLook.m_YAxis.m_InputAxisValue = PlayerInput.Instance.CameraInput.y;
        }
    }
}