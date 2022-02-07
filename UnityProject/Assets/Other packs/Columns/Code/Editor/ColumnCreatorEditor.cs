using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(ColumnCreator))]
public class ColumnCreatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ColumnCreator cc = (ColumnCreator)target;
        
        if (GUILayout.Button("Build Object"))
        {
            cc.SpawnDefaultColumn();
        }
    }
}

