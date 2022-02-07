using UnityEngine;

namespace Game.Weapon.ProjectileLauncher.Interfaces
{
    public interface IProjectileLauncher
    {
        LaunchProjectileResponse Launch(LaunchProjectileRequest request);
        bool canShoot { get; }
    }

    public class LaunchProjectileRequest
    {
        public readonly GameObject prefab;
        public readonly float damage;

        public LaunchProjectileRequest(GameObject prefab, float damage)
        {
            this.prefab = prefab;
            this.damage = damage;
        }
    }

    public class LaunchProjectileResponse
    {
        public LaunchProjectileResponse(bool success)
        {
            Success = success;
        }

        public bool Success { get; }
    }
}