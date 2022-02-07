using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;

public class EditorGameSettingsWindow : OdinEditorWindow
{
    [MenuItem("EditorTools/Windows/EditorGameSettings")]
    public static void OpenWindow()
    {
        GetWindow<EditorGameSettingsWindow>().Show();
    }

    protected override object GetTarget()
    {
        return EditorGameSettings.Instance;
    }
}