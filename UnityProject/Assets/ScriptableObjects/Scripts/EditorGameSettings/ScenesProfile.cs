using System.Collections.Generic;

namespace CustomeEditorTools.EditorGameSettingsData
{
    [System.Serializable]
    public class ScenesProfile
    {
        public UnityEngine.Object coreScene;
        public UnityEngine.Object loadingScene;
        public List<UnityEngine.Object> environmentScenes;
    }
}