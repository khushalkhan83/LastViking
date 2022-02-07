using UnityEditor;
using UnityEngine;


public class EditorOnlySettings : ScriptableObject
{
    #if UNITY_EDITOR
    [MenuItem("EditorTools/Inspector/Switch sse rerodable list")]
    private static void SelectEditorGameSettings()
    {
        instance.useReordableLists = !instance.useReordableLists;
    }
    #endif
    
    #region Singltone

    private static EditorOnlySettings instance;
    public static EditorOnlySettings Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<EditorOnlySettings>("SO_Settings_EditorOnly");
            }
            return instance;
        }
    }

    #endregion

    public bool useReordableLists;
}