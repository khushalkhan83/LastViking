using System;
using AddressablesRelated;
using Game.Providers;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Game.Models
{
    public class WeaponAssetsLoadingModel : MonoBehaviour
    {
        [SerializeField] private StaticPlayerWeaponProvider staticPlayerWeaponProvider;
        private static AddressablePool weaponPool;
        public AddressablePool WeaponPool {
            get
            {
                if(weaponPool == null) weaponPool = new AddressablePool();
                return weaponPool;
            }
        }
        public StaticPlayerWeaponProvider StaticPlayerWeaponProvider => staticPlayerWeaponProvider;
        
        public void Refresh() => weaponPool.Refresh();
    }
}
