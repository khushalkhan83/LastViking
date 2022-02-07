using UnityEngine;
namespace Game.Models
{
    public enum PlayerTargetItem
    {
        Buildings,
        Animals,
        Wood,
        Stone,
        MetalOre,
        Lootbox,
        Enemy,
    }

    public interface IDamageable
    {
        void Damage(float value, GameObject from = null);
    }
}
