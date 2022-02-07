using Core.Providers;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_materials_default", menuName = "Providers/Materials", order = 0)]

    public class MaterialsProvider : ProviderScriptable<MaterialID, Material>
    {
        [EnumNamedArray(typeof(MaterialID))]
        [SerializeField] private Material[] _data;

        public override Material this[MaterialID key] => _data[((int)(object)key - 1)];
    }
}
