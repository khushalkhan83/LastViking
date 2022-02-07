using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TreasureHuntPlace))]
public class TreasureHuntPlaceEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("ProjectMesh"))
        {
            (target as TreasureHuntPlace).BakeMesh();
        }
    }
}
