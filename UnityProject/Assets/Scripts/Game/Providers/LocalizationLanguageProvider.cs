using Core.Providers;
using Game.Models;
using UnityEditor;
using UnityEngine;

namespace Game.Providers
{
    public class LocalizationLanguageProvider : ProviderScriptable<LocalizationLanguageID, LocalizationDataProvider> {
        [EnumNamedArray(typeof(LocalizationLanguageID))]
        [SerializeField] private LocalizationDataProvider[] _data;

        public override LocalizationDataProvider this[LocalizationLanguageID key] => _data[((int)(object)key - 1)];


#if UNITY_EDITOR
        public LocalizationDataProvider[] Data => _data;
        public void Init(LocalizationDataProvider[] values) 
        { 
            _data = values;
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
#endif
    }
}
