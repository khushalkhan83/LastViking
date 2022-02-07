using Core.Providers;
using Game.Models;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_sceneNames_default", menuName = "Providers/SceneNames", order = 0)]

    public class SceneNamesProvider : ProviderScriptable<EnvironmentSceneID, string> {
        [EnumNamedArray(typeof(EnvironmentSceneID))]
        [SerializeField] private string[] _data;

        public override string this[EnvironmentSceneID key] => _data[((int)(object)key - 1)];
        
        public string[] Data => _data;

        #if UNITY_EDITOR
        public void ExpandAndAddSceneName(string sceneName)
        {
            var oldData = _data;
            _data = new string[_data.Length+1];

            for (int i = 0; i < oldData.Length; i++)
            {
                _data[i] = oldData[i];
            }

            _data[_data.Length - 1] = sceneName;
        }

        #endif
     }
}