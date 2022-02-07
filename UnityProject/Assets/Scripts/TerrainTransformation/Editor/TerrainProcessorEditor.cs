using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TerrainProcessor))]
public class TerrainProcessorEditor : Editor
{
    public TerrainProcessor Target => (TerrainProcessor)target;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var colorBackgroundDefault = GUI.backgroundColor;

        GUILayout.BeginHorizontal(GUI.skin.box);

        GUI.backgroundColor = new Color(0.7f, 0.7f, 0.7f);

        if (GUILayout.Button("Clear"))
        {
            Clear();
        }

        GUI.backgroundColor = new Color(0, 0.95f, 0.8f);
        if (GUILayout.Button("Save"))
        {
            Save();
        }

        if (GUILayout.Button("Restore"))
        {
            Resote();
        }

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Bake", GUILayout.ExpandWidth(true)))
        {
            Bake();
        }
        GUI.backgroundColor = colorBackgroundDefault;

        GUILayout.EndHorizontal();
    }

    private void Clear()
    {
        Target.Clear();
    }

    private void Save()
    {
        var path = EditorUtility.SaveFilePanelInProject("Save terrain trees", Target.Terrain.terrainData.name + "Trees", "asset", "Choise folder and save trees from terrain to file");

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        var asset = CreateInstance<TerrainSave>();

        asset.Trees = Target.Terrain.terrainData.treeInstances;

        AssetDatabase.CreateAsset(asset, path);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }

    private void Resote()
    {
        var path = EditorUtility.OpenFilePanelWithFilters("restore trees", "", new[] { "Assets", "asset" });

        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        const string assetsFolder = "Assets/";
        var index = path.IndexOf(assetsFolder);
        path = path.Remove(0, index);

        var asset = AssetDatabase.LoadAssetAtPath<TerrainSave>(path);

        if (!asset)
        {
            return;
        }

        Target.Terrain.terrainData.treeInstances = asset.Trees;

        Selection.activeObject = Target;
    }

    private void Bake()
    {
        Target.ReplaseObjects();
    }
}
