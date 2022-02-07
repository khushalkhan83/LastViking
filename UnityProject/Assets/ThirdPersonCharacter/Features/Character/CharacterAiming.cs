using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ThirdPerson
{
    public class CharacterAiming : MonoBehaviour
    {
        public float turnSpeed = 15;
        UnityEngine.Camera mainCamera;
        
        void Start()
        {
            mainCamera = UnityEngine.Camera.main;
        }

        void Update()
        {
            float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.deltaTime);
        }
    }
}
