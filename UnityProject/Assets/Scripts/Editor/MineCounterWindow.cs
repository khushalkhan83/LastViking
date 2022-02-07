using System.Linq;
using UltimateSurvival;
using UnityEditor;
using UnityEngine;

public class MineCounterWindow : EditorWindow
{
    [MenuItem("Tools/Resources Counter")]
    public static void OpenWindow()
    {
        GetWindow<MineCounterWindow>(true);
    }

    private void OnGUI()
    {
        var mines = FindObjectsOfType<MineableObject>();

        var group = mines.GroupBy(x => x.RequiredToolPurpose);

        foreach (var item in group)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label(item.Key.ToString());
            GUILayout.FlexibleSpace();
            GUILayout.Label(item.Sum(x => x.Amount).ToString());
            GUILayout.EndHorizontal();
        }
    }
}
