using Core.Providers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_staticPlayerWeapon_default", menuName = "Providers/StaticPlayerWeapon", order = 0)]

    public class StaticPlayerWeaponProvider : ProviderScriptable<PlayerWeaponID, FPObject> {
        [EnumNamedArray(typeof(PlayerWeaponID))]
        [SerializeField] private FPObject[] _data;

        public override FPObject this[PlayerWeaponID key] => _data[((int)(object)key - 1)];

        public bool Contains(PlayerWeaponID key) => _data.IndexIsValid(((int)(object)key - 1)) && _data[((int)(object)key - 1)] != null;
     }
}
