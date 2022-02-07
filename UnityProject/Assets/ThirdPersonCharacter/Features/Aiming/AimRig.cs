using UnityEngine;

namespace Game.ThirdPerson.RangedCombat.Misc
{
    public class AimRig : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float xOverride;
        [SerializeField] private float yOverride;

        private void Start()
        {
            var rotation = transform.eulerAngles;
            rotation.y -= 180;
            transform.eulerAngles = rotation;
        }
        private void Update()
        {
            var rotation = transform.eulerAngles;
            rotation.x = -target.eulerAngles.x + xOverride;
            rotation.y = target.eulerAngles.y + yOverride;
            transform.eulerAngles = rotation;
        }
    }
}