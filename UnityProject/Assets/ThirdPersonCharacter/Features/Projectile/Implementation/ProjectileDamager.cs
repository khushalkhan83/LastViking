using Game.AI;
using Game.Models;
using UnityEngine;

namespace Game.Weapon.ProjectileLauncher.Implementation
{
    public class ProjectileDamager : MonoBehaviour
    {
        public float damage;
        public GameObject from;

        public void OnCollisionEnter(Collision other)
        {
            Debug.Log(other.gameObject.name);

            IDamageable damagable = other.transform.GetComponentInParent<IDamageable>();

            if(damagable == null)
            {
                var health = other.gameObject.GetComponentInParent<IHealth>();
                if (health != null)
                {
                    damagable = other.transform.parent.GetComponentInChildren<IDamageable>();
                }
            }

            if(damagable != null) damagable.Damage(damage, from);
        }
    }
}