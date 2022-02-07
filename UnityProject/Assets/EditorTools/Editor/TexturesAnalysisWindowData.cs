using UnityEngine;
using NaughtyAttributes;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;

namespace CustomeEditorTools
{
    namespace TextureAnalysis
    {
        public enum Platform { Android, IOS }


        public enum TextureTypeFilter { all, sprites, notSprites }
        public enum MipMapFilter { none, onlyMipMap, ignoreMipMap }
        public enum FilesFilter { none, onlyValuesFromReport, ignoreValuesFromReport }
        public enum ShowType { splitByMemory, splitByFolders }

        // [CreateAssetMenu(fileName = "TexturesAnalysisWindowData", menuName = "UnityProject/TexturesAnalysisWindowData", order = 0)]
        public class TexturesAnalysisWindowData : ScriptableObject
        {
            #region Singltone

            private static TexturesAnalysisWindowData instance;
            public static TexturesAnalysisWindowData Instance
            {
                get
                {
                    if (instance == null)
                    {
                        instance = Resources.Load<TexturesAnalysisWindowData>("SO_TexturesAnalysisWindowData");
                    }
                    return instance;
                }
            }

            #endregion

            public Platform SelectedPlatform;

            public TextureTypeFilter SelectedTextureTypeFilter;
            public MipMapFilter SelectedMipMapFilter;
            public FilesFilter SelectedFilesFilter;
            public ShowType SelectedShowType;

            [ResizableTextArea]
            [SerializeField] private string report;

            public List<string> GetReportFilesNames()
            {

                string[] lines = report.Split(
                    new[] { Environment.NewLine },
                    StringSplitOptions.None
                );
                return lines.ToList();
            }

            [SerializeField] private List<string> testReport;
            [SerializeField] private List<ImportedTextureData> testImportedTextureDatas;

            
            [Button] void TestReport() => testReport = GetReportFilesNames();
            [Button] void TestImportedTextureData() => testImportedTextureDatas = GetImportedTextureDatas();


            public List<ImportedTextureData> GetImportedTextureDatas()
            {
                var answer = new List<ImportedTextureData>();
                var lines = GetReportFilesNames();

                foreach (var line in lines)
                {
                    string[] result = line.Split('\t');

                    var d = new ImportedTextureData()
                    {
                        name = result.Length == 2 ? result[0] : "",
                        size = result.LastOrDefault(),
                    };
                    answer.Add(d);
                }

                return answer;
            }

        }

        [System.Serializable]
        public class ImportedTextureData
        {
            public string name;
            public string size;
        }
    }
}
