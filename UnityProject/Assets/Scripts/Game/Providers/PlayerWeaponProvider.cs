using Core.Providers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_playerWeapon_default", menuName = "Providers/PlayerWeapon", order = 0)]

    public class PlayerWeaponProvider : ProviderScriptable<PlayerWeaponID, AssetReference> {
        // [EnumNamedArray(typeof(PlayerWeaponID))]
        [SerializeField] private AssetReference[] _data;

        public override AssetReference this[PlayerWeaponID key] => _data[((int)(object)key - 1)];


        #if UNITY_EDITOR

        public AssetReference[] Data => _data;
        
        #endif
     }
}
