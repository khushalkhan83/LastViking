using Core.Providers;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_prefabs_default", menuName = "Providers/Prefabs", order = 0)]

    public class PrefabsProvider : ProviderScriptable<PrefabID, GameObject> {
        [EnumNamedArray(typeof(PrefabID))]
        [SerializeField] private GameObject[] _data;

        public override GameObject this[PrefabID key] => _data[((int)(object)key - 1)];
     }
}
