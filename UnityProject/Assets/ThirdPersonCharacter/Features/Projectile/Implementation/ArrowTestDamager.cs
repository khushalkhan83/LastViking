using Game.VillageBuilding;
using UnityEngine;

namespace Game.Weapon.ProjectileLauncher.Implementation
{
    public class ArrowTestDamager : MonoBehaviour
    {
        public void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Player") return;
            
            var dammageEffect = collision.gameObject.GetComponentInParent<DamagedEffectAnimation>();

            if(dammageEffect == null) return;

            dammageEffect.PlayAnimation();
        }
    }
}