using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace CustomeEditorTools
{
    namespace TextureAnalysis
    {

        public class TexturesAnalysisWindow : EditorWindow
        {

            private TexturesAnalysisWindowData Storage => TexturesAnalysisWindowData.Instance;

            private List<string> cashedReportFilesNames;
            private List<ImportedTextureData> cashedImportedTextureData;
            private List<TextureData> texturesOutput_2k = new List<TextureData>();
            private List<TextureData> texturesOutput_1k = new List<TextureData>();
            private List<TextureData> texturesOutput_512 = new List<TextureData>();
            private List<TextureData> texturesOutput_smaller = new List<TextureData>();

            public class TextureData
            {
                public string name;
                public float maxTextureSizePerPlatform;
                public string buildSizeInMb;
                public string folderPath;
                public Texture texture;

                public string Lable()
                {
                    return name + " size: " + ((buildSizeInMb == "") ? "___" : buildSizeInMb.ToString());
                }
            }

            public class TextureFolderData { 
                public float totalSize;
                public bool show;
            }


            private Vector2 scrollPos;
            private bool show_2k;
            private bool show_1k;
            private bool show_512;
            private bool show_smaller;
            private Dictionary<string, TextureFolderData> showFolders = new Dictionary<string, TextureFolderData>();

            [MenuItem("EditorTools/Windows/Utils/TexturesAnalysis")]
            private static void ShowWindow()
            {
                var window = GetWindow<TexturesAnalysisWindow>();
                window.titleContent = new GUIContent("TexturesAnalysisWindow");
                window.Show();
            }

            private void OnGUI()
            {
                GUILayout.BeginHorizontal();

                GUILayout.BeginVertical();
                Storage.SelectedPlatform = (Platform)EditorGUILayout.EnumPopup("Platform:", Storage.SelectedPlatform);
                Storage.SelectedTextureTypeFilter = (TextureTypeFilter)EditorGUILayout.EnumPopup("TextureTypeFilter:", Storage.SelectedTextureTypeFilter);
                Storage.SelectedMipMapFilter = (MipMapFilter)EditorGUILayout.EnumPopup("MipMapFilter:", Storage.SelectedMipMapFilter);
                Storage.SelectedFilesFilter = (FilesFilter)EditorGUILayout.EnumPopup("FilesFilter:", Storage.SelectedFilesFilter);
                GUILayout.Space(10);
                Storage.SelectedShowType = (ShowType)EditorGUILayout.EnumPopup("ShowType:", Storage.SelectedShowType);
                GUILayout.EndVertical();

                GUILayout.BeginVertical();
                if (GUILayout.Button("Analise"))
                {
                    cashedReportFilesNames = Storage.GetReportFilesNames();
                    cashedImportedTextureData = Storage.GetImportedTextureDatas();
                    Analysise();
                }
                if (GUILayout.Button("Print All")) PringAll();
                if (GUILayout.Button("Print splited")) PringSplited();
                GUILayout.EndVertical();

                GUILayout.EndHorizontal();




                GUILayout.Space(15);

                var oldColor = GUI.color;
                var buttonCollor = new Color(0.4862745f, 0.6705883f, 0.9450981f);

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.MaxWidth(2000), GUILayout.MaxHeight(2000));
                if (Storage.SelectedShowType == ShowType.splitByMemory)
                {
                    show_2k = EditorGUILayout.Foldout(show_2k, "2k");
                    if (show_2k)
                        foreach (var item in texturesOutput_2k)
                        {
                            GUI.color = buttonCollor;
                            if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                            GUI.color = oldColor;
                        }
                    show_1k = EditorGUILayout.Foldout(show_1k, "1k");
                    if (show_1k)
                        foreach (var item in texturesOutput_1k)
                        {
                            GUI.color = buttonCollor;
                            if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                            GUI.color = oldColor;
                        }
                    show_512 = EditorGUILayout.Foldout(show_512, "512");
                    if (show_512)
                        foreach (var item in texturesOutput_512)
                        {
                            GUI.color = buttonCollor;
                            if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                            GUI.color = oldColor;
                        }
                    show_smaller = EditorGUILayout.Foldout(show_smaller, "smaller");
                    if (show_smaller)
                        foreach (var item in texturesOutput_smaller)
                        {
                            GUI.color = buttonCollor;
                            if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                            GUI.color = oldColor;
                        }
                }
                else 
                {
                    foreach (var key in showFolders.Keys) {
                        string folderName = key + "  Size: " + showFolders[key].totalSize + "Mb";
                        showFolders[key].show = EditorGUILayout.Foldout(showFolders[key].show, folderName);
                        if (showFolders[key].show) {
                            foreach (var item in texturesOutput_2k) {
                                if (item.folderPath == key)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUI.color = buttonCollor;
                                    if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                                    GUI.color = oldColor;
                                    GUILayout.EndHorizontal();
                                }
                            }
                            foreach (var item in texturesOutput_1k)
                            {
                                if (item.folderPath == key)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUI.color = buttonCollor;
                                    if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                                    GUI.color = oldColor;
                                    GUILayout.EndHorizontal();
                                }
                            }
                            foreach (var item in texturesOutput_512)
                            {
                                if (item.folderPath == key)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUI.color = buttonCollor;
                                    if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                                    GUI.color = oldColor;
                                    GUILayout.EndHorizontal();
                                }
                            }
                            foreach (var item in texturesOutput_smaller)
                            {
                                if (item.folderPath == key)
                                {
                                    GUILayout.BeginHorizontal();
                                    GUILayout.Space(20);
                                    GUI.color = buttonCollor;
                                    if (GUILayout.Button(item.Lable(), GUI.skin.label)) Selection.activeObject = item.texture;
                                    GUI.color = oldColor;
                                    GUILayout.EndHorizontal();
                                }
                            }
                        }
                    }
                }


                GUILayout.EndScrollView();
            }

            private void PringAll()
            {
                StringBuilder sb = new StringBuilder();

                if (Storage.SelectedShowType == ShowType.splitByMemory)
                {
                    texturesOutput_2k.ForEach(x => sb.AppendLine(x.name));
                    texturesOutput_1k.ForEach(x => sb.AppendLine(x.name));
                    texturesOutput_512.ForEach(x => sb.AppendLine(x.name));
                    texturesOutput_smaller.ForEach(x => sb.AppendLine(x.name));
                }
                else if(Storage.SelectedShowType == ShowType.splitByFolders){
                    foreach (var key in showFolders.Keys)
                    {
                        string folderName = key + "  Size: " + showFolders[key].totalSize + "Mb";
                        sb.Append(folderName);
                        foreach (var item in texturesOutput_2k)
                        {
                            if (item.folderPath == key)
                            {
                                sb.AppendLine(item.Lable().Replace(" ", "\t"));
                            }
                        }
                        foreach (var item in texturesOutput_1k)
                        {
                            if (item.folderPath == key)
                            {
                                sb.AppendLine(item.Lable().Replace(" ", "\t"));
                            }
                        }
                        foreach (var item in texturesOutput_512)
                        {
                            if (item.folderPath == key)
                            {
                                sb.AppendLine(item.Lable().Replace(" ", "\t"));
                            }
                        }
                        foreach (var item in texturesOutput_smaller)
                        {
                            if (item.folderPath == key)
                            {
                                sb.AppendLine(item.Lable().Replace(" ", "\t"));
                            }
                        }
                    }
                }

                Debug.Log(sb.ToString());
            }
            private void PringSplited()
            {
                StringBuilder sb = new StringBuilder();

                texturesOutput_2k.ForEach(x => sb.AppendLine(x.name));
                Debug.Log(sb.ToString());
                sb.Clear();

                texturesOutput_1k.ForEach(x => sb.AppendLine(x.name));
                Debug.Log(sb.ToString());
                sb.Clear();

                texturesOutput_512.ForEach(x => sb.AppendLine(x.name));
                Debug.Log(sb.ToString());
                sb.Clear();

                texturesOutput_smaller.ForEach(x => sb.AppendLine(x.name));
                Debug.Log(sb.ToString());
                sb.Clear();
            }

            private void Analysise()
            {

                texturesOutput_2k.Clear();
                texturesOutput_1k.Clear();
                texturesOutput_512.Clear();
                texturesOutput_smaller.Clear();
                showFolders.Clear();

                foreach (string guid in AssetDatabase.FindAssets("t:texture", null))
                {
                    string path = AssetDatabase.GUIDToAssetPath(guid);
                    TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                    Texture texture = AssetDatabase.LoadAssetAtPath(path, typeof(Texture)) as Texture;
                   
                    if (textureImporter != null)
                    {
                        
                        var platformTextureData = textureImporter.GetPlatformTextureSettings(Storage.SelectedPlatform.ToString());
                        bool ignore = ConditionsFilter(textureImporter,texture.name);
                        if (ignore) continue;


                        var matchInImportedFiles = cashedImportedTextureData.Find(x => x.name == texture.name);

                        var buildTextureSizeInMb = "";
                        if(matchInImportedFiles != null)
                        {
                            buildTextureSizeInMb = matchInImportedFiles.size;
                        }

                        int lastSlashIndex = path.LastIndexOf('/');
                        string folderPath = path.Substring(0, lastSlashIndex);
                        if (!showFolders.ContainsKey(folderPath))
                        {
                            showFolders[folderPath] = new TextureFolderData();
                        }
                        if (buildTextureSizeInMb != "")
                        {
                            showFolders[folderPath].totalSize += (float)Convert.ToDouble(buildTextureSizeInMb.Replace("Mb", ""));
                        }


                        var output = GetDictionary(textureImporter.maxTextureSize);
                        output.Add(
                            new TextureData() { 
                                name = texture.name,
                                maxTextureSizePerPlatform = platformTextureData.maxTextureSize,
                                texture = texture,
                                buildSizeInMb = buildTextureSizeInMb,
                                folderPath = folderPath,
                                }
                        );
                    }
                }
                showFolders = showFolders.OrderByDescending(x => x.Value.totalSize).ToDictionary(x => x.Key, x => x.Value);
                texturesOutput_2k = texturesOutput_2k.OrderByDescending(d => d.buildSizeInMb).ToList();
                texturesOutput_1k = texturesOutput_1k.OrderByDescending(d => d.buildSizeInMb).ToList();
                texturesOutput_512 = texturesOutput_512.OrderByDescending(d => d.buildSizeInMb).ToList();
                texturesOutput_smaller = texturesOutput_smaller.OrderByDescending(d => d.buildSizeInMb).ToList();

                List<TextureData> GetDictionary(int size)
                {
                    switch (size)
                    {
                        case 2048:
                            return texturesOutput_2k;
                        case 1024:
                            return texturesOutput_1k;
                        case 512:
                            return texturesOutput_512;
                        default:
                            return texturesOutput_smaller;
                    }
                }
            }

            ImportedTextureData tempTextureData;
            bool foundFileWithSameName;
            private bool ConditionsFilter(TextureImporter textureImporter, string fileName)
            {   
                bool ignore = false;

                switch (Storage.SelectedMipMapFilter)
                {
                    case MipMapFilter.none:
                        break;
                    case MipMapFilter.ignoreMipMap:
                        if (textureImporter.mipmapEnabled)
                        {
                            ignore = true;
                            return ignore;
                        }
                        break;
                    case MipMapFilter.onlyMipMap:
                        if (!textureImporter.mipmapEnabled)
                        {
                            ignore = true;
                            return ignore;
                        }
                        break;
                    default:
                        break;
                }

                switch (Storage.SelectedTextureTypeFilter)
                {
                    case TextureTypeFilter.all:
                        break;
                    case TextureTypeFilter.notSprites:
                        if (textureImporter.textureType == TextureImporterType.Sprite)
                        {
                            ignore = true;
                            return ignore;
                        }
                        break;
                    case TextureTypeFilter.sprites:
                        if (textureImporter.textureType != TextureImporterType.Sprite)
                        {
                            ignore = true;
                            return ignore;
                        }
                        break;
                    default:
                        break;
                }


                switch (Storage.SelectedFilesFilter)
                {
                    case FilesFilter.none:
                        break;
                    case FilesFilter.ignoreValuesFromReport:
                        tempTextureData = cashedImportedTextureData.Find(x => x.name == fileName);
                        foundFileWithSameName = tempTextureData != null;
                        if(foundFileWithSameName)
                        {
                            ignore = true;
                            return ignore;
                        }
                        break;
                    case FilesFilter.onlyValuesFromReport:
                        tempTextureData = cashedImportedTextureData.Find(x => x.name == fileName);
                        foundFileWithSameName = tempTextureData != null;
                        if(!foundFileWithSameName)
                        {
                            ignore = true;
                            return ignore;
                        }
                        break;
                    default:
                        break;
                }
                return ignore;
            }

            // TODO: add via Reflection ....
            // private float GetTextureCompresedSizeInMb(TextureImporter textureImporter)
            // {
                // EditorUtility.FormatBytes(TextureUtil.GetStorageMemorySizeLong(t));
            // }
        }
    }
}