using Game.Models;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(StorageModel))]
public class StorageModelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        if (GUILayout.Button("Clear"))
        {
            (target as StorageModel).ClearAll();
        }
    }
}
