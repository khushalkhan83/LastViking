using System.Collections;
using UnityEngine;

namespace Game.Weapon.ProjectileLauncher.Implementation
{
    public class ArrowProjectile : MonoBehaviour
    {
        public float force;
        public Rigidbody rigidbody;
        public Collider collider;
        Cinemachine.CinemachineImpulseSource source;
        private void Awake()
        {
            rigidbody = GetComponent<Rigidbody>();
            collider = GetComponent<Collider>();
            rigidbody.centerOfMass = transform.position;
        }

        private void Update() 
        {
            if(rigidbody.velocity != Vector3.zero)
            {
                transform.forward = rigidbody.velocity;
            }
        }

        public void Fire()
        {
            rigidbody.AddForce(transform.forward * 220f, ForceMode.Impulse);
            source = GetComponent<Cinemachine.CinemachineImpulseSource>();

            source.GenerateImpulse(Camera.main.transform.forward);
        }

        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name != "Player")
            {
                rigidbody.isKinematic = true;
                Debug.Log("Collision " + collision.gameObject.name);
                StartCoroutine(Countdown());
                collider.enabled = false;
                transform.parent = collision.transform.parent;
            }
        }

        private IEnumerator Countdown()
        {
            yield return new WaitForSeconds(10);
            Destroy(gameObject);
        }
    }
}