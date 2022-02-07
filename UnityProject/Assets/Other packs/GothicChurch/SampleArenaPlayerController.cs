using UnityEngine;

namespace FullInspector.Samples.Games.Arena
{
    public class SampleArenaPlayerController : MonoBehaviour
    {
        //("The speed that the player moves at")
        public float Speed = 1;

        protected void Update()
        {
            //move forward/backward
            transform.Translate(Input.GetAxis("Vertical") * Vector3.forward * Speed * Time.deltaTime);
            transform.Translate(Input.GetAxis("Horizontal") * Vector3.right * Speed * Time.deltaTime);
        }
    }
}