using System.Collections.Generic;
using Extensions;
using UnityEngine;

namespace CustomeEditorTools.CodeGenerator
{
    // [CreateAssetMenu(fileName = "SO_Data_CodeGeneratorWindow", menuName = "GeneratorData/CodeGeneratorWindowData", order = 0)]
    public class CodeGeneratorWindowData : ScriptableObject
    {
        [Header("Layout")]
        public float sideBarMaxWidth = 100;
        public float maxToolBarButtonWidth = 100;
        public float toolBarMaxWidth = 1000;


        [Header("Main settings")]
        public List<CodeGeneratorBase> codeGenerators = new List<CodeGeneratorBase>();
        public GlobalSettings globalSettings;
        

        [Header("Documentation")]
        public string documentationURL;

        public int LastSelectedElementIndex { get; set; } = -1;


        private static CodeGeneratorWindowData instance;

        public static CodeGeneratorWindowData Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<CodeGeneratorWindowData>("SO_Data_CodeGeneratorWindow");
                }
                return instance;
            }
        }



        [System.Serializable]
        public class GlobalSettings
        {
            [SerializeField] private UnityEngine.Object injectionSystemFile;
            [SerializeField] private UnityEngine.Object modelsPrefab;

            public string InjectionSystemFilePath => injectionSystemFile.Path();
            public string ModelsPrefabPath => modelsPrefab.Path();
        }
    }
}