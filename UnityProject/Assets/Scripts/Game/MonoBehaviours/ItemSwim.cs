using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSwim : MonoBehaviour
{
    [SerializeField] float upwardForce = 12.72f; // 9.81 is the opposite of the default gravity, which is 9.81. If we want the boat not to behave like a submarine the upward force has to be higher than the gravity in order to push the boat to the surface
    [SerializeField] private Rigidbody rb;
    [SerializeField] DisablePhysicsAfterTime disablePhysicsAfterTime;
    [SerializeField] private float waveringAmplitude = 0.5f;
    [SerializeField] private float waveringSpeed = 1f;

    private const int waterLayerNum = 4;
    private bool isInWater = false;
    private bool isOnTopWater = false;
    private float degrees = 0;
    private float topWaterY = 0;

    void OnTriggerEnter(Collider collidier)
    {
        if (isOnTopWater || collidier.gameObject.layer != waterLayerNum) return;

        if (disablePhysicsAfterTime != null)
        {
            disablePhysicsAfterTime.enabled = false;
        }

        isInWater = true;
        rb.drag = 5f;
    }

    void OnTriggerExit(Collider collidier)
    {
        if (isOnTopWater || collidier.gameObject.layer != waterLayerNum) return;

        isInWater = false;
        isOnTopWater = true;
        rb.isKinematic = true;
        topWaterY = transform.position.y;
    }


    void FixedUpdate()
    {
        if (isInWater)
        {
            this.rb.AddForce(Vector3.up * upwardForce, ForceMode.Acceleration);
        }
        else if (isOnTopWater) 
        {
            float y = Mathf.Sin(degrees) * waveringAmplitude;
            transform.position = new Vector3(transform.position.x, topWaterY + y, transform.position.z);
            degrees += Time.deltaTime * waveringSpeed;
        }
    }
}
