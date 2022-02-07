using Core.Providers;
using Game.Models;
using UnityEngine;

namespace Game.Providers
{
    public class LocalizationDataProvider : ProviderScriptable<LocalizationKeyID, string>
    {
        [EnumNamedArray(typeof(LocalizationKeyID))]
        [SerializeField] private string[] _data;

        public override string this[LocalizationKeyID key] => _data[((int)(object)key - 1)];

#if UNITY_EDITOR
        public void Init(string[] values) => _data = values;
        public string[] Data => _data;
#endif
    }
}
