using Cinemachine;
using UnityEngine;

public class PlayerCameras : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private Camera _cameraWorld;
    [SerializeField] private CinemachineFreeLook _camera;

#pragma warning restore 0649
    #endregion

    public Camera CameraWorld => _cameraWorld;
    public CinemachineFreeLook Camera=>_camera;

    public void ShowAllCameras() => SetIsVisibleCameras(true);
    public void HideAllCameras() => SetIsVisibleCameras(false);

    public void SetIsVisibleCameras(bool isVisible)
    {
        Camera.gameObject.SetActive(isVisible);
        CameraWorld.gameObject.SetActive(isVisible);
    }
}
