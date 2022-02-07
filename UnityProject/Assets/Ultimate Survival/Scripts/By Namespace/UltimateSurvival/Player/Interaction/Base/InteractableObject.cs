using UnityEngine;

namespace UltimateSurvival
{
    public class RaycastData
    {
        public bool ObjectIsInteractable { get; private set; }
        public Ray CameraRay { get; private set; }
        public RaycastHit HitInfo { get; private set; }
        public GameObject GameObject { get; private set; }
        public InteractableObject InteractableObject { get; private set; }

        public static implicit operator bool(RaycastData raycastData)
        {
            return raycastData != null;
        }

        public RaycastData()
        {

        }

        public void SetData(RaycastHit hitInfo, Ray ray)
        {
            GameObject = hitInfo.collider.gameObject;
            InteractableObject = hitInfo.collider.GetComponent<InteractableObject>();

            if (InteractableObject && !InteractableObject.enabled)
            {
                InteractableObject = null;
            }

            ObjectIsInteractable = InteractableObject != null;
            HitInfo = hitInfo;
            CameraRay = ray;
        }
    }

    public class InteractableObject : MonoBehaviour
    {
    }
}