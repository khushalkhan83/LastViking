using UnityEngine;

public class PlayerCameras : MonoBehaviour
{
    #region Data
#pragma warning disable 0649

    [SerializeField] private Camera _cameraWorld;
    [SerializeField] private Camera _cameraTools;
    [SerializeField] private Camera _cameraBackground;

#pragma warning restore 0649
    #endregion

    public Camera CameraWorld => _cameraWorld;
    public Camera CameraTools => _cameraTools;
    public Camera CameraBackground => _cameraBackground;

    public void ShowAllCameras() => SetIsVisibleCameras(true);
    public void HideAllCameras() => SetIsVisibleCameras(false);

    public void SetIsVisibleCameras(bool isVisible)
    {
        CameraWorld.gameObject.SetActive(isVisible);
        CameraTools.gameObject.SetActive(isVisible);
        CameraBackground.gameObject.SetActive(isVisible);
    }
}
