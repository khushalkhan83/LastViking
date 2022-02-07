using UnityEngine;
using UnityEditor;

// [CreateAssetMenu(fileName = "SO_Settings_Editor", menuName = "EditorGameSettings/EditorGameSettings", order = 0)]
public partial class EditorGameSettings : ScriptableObject
{
    #region Singltone

    private static EditorGameSettings instance;
    public static EditorGameSettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<EditorGameSettings>("SO_Settings_Editor");
            }
            return instance;
        }
    }

    #endregion


    #if UNITY_EDITOR
    private bool EditorInPlayMode()
    {
        return EditorApplication.isPlaying;
    }
    #endif
}