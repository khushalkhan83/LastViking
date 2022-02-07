
using System;
using Extensions;
using Game.Views;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools.CodeGenerator
{
    [CreateAssetMenu(fileName = "ViewCodeGenerator", menuName = "GeneratorData/ViewCodeGenerator", order = 0)]
    public class ViewCodeGenerator : CodeGeneratorBase
    {
        // TODO: show to user that when create viewController view name will be used
        [SerializeField] private bool createView;
        [SerializeField] private bool createViewController;
        [SerializeField] private string viewName;
        [SerializeField] private string viewControllerName;
        [SerializeField] private string viewIDName;
        [SerializeField] private string viewConfigIDName;
        [SerializeField] private LayerID layerID = LayerID.Base;
        [SerializeField] private Settings settings;

        [Space]
        [SerializeField] private UnityEngine.Object lastCreatedPrefab;
        [Button] void STEP_1_2_3()
        {
            GenerateScripts(false);
            Add_IDS(false);
            MapIDInViewMapper(false);

            AssetDatabase.Refresh();
            CacheCreatedFiles();
        }
        [Button] void STEP_4_5()
        {
            CreateViewPrefabAndCache();
            MapViewPrefabToViewSystem();
        }
        [Button] void STEP_1_Generate_Scripts() {GenerateScripts(true); CacheCreatedFiles();}
        [Button] void STEP_2_Add_IDS() => Add_IDS(true);
        [Button] void STEP_3_MapIDInViewMapper() => MapIDInViewMapper(true);
        [Button] void STEP_4_CreateViewPrefab() => CreateViewPrefabAndCache();
        [Button] void STEP_5_MapViewPrefabToViewSystem() => MapViewPrefabToViewSystem();


        private string ViewPath => FullPath(settings.ViewsFolderPath, viewName);
        private string ViewControllerPath => FullPath(settings.ViewControllersFolderPath, viewControllerName);

        private void GenerateScripts(bool refresh)
        {
            var dataToCheck = new CodeGeneratorBase.FilePathData[]{
                new CodeGeneratorBase.FilePathData(ViewPath,createView),
                new CodeGeneratorBase.FilePathData(ViewControllerPath,createViewController),
            };

            bool someFilesExist = CheckForFileExistErrors(dataToCheck);

            if (someFilesExist)
            {
                throw new System.Exception("Some files exist");
            }

            if(createView) CreateScript(ViewPath, ViewFile());
            if(createViewController) CreateScript(ViewControllerPath, ViewControllerFile());

            if (refresh) AssetDatabase.Refresh();
        }


        private void CacheCreatedFiles()
        {
            CacheCreatedFile(ViewPath);
            CacheCreatedFile(ViewControllerPath);
            AssetDatabase.SaveAssets();
        }

        private void Add_IDS(bool refresh)
        {
            Add_ViewID(false);
            Add_ViewConfigID(refresh);
        }

        private void Add_ViewID(bool refresh) => AddIDToExistingFile(
            refresh,
            settings.ViewIDFilePath,
            viewIDName,
            "///CODE_GENERATION_VIEW_ID",
            "        ");

        private void Add_ViewConfigID(bool refresh) => AddIDToExistingFile(
            refresh,
            settings.ViewConfigIDFilePath,
            viewConfigIDName,
            "///CODE_GENERATION_VIEW_CONFIG_ID",
            "        ");

        // TODO: move to base class and reuse in controller generator
        private void AddIDToExistingFile(bool refresh, string filePath, string newIDName, string tag, string space)
        {
            bool error = !TryGetClosestNumInLineAboveTag(tag, filePath, out int lastID);

            if (error)
            {
                Debug.LogError("Error here");
                return;
            }

            AddTextAboveTag(tag, filePath, GetNewIDLine(), space);
            if (refresh) AssetDatabase.Refresh();

            string GetNewIDLine()
            {
                var idName = newIDName;
                var id = lastID + 1;
                var answer = idName + " = " + id + ",";
                return answer;
            }
        }

        private void MapIDInViewMapper(bool refresh) => AddIDToViewMapper(
            refresh,
            settings.ViewsMapperPath,
            viewIDName,
            "///CODE_GENERATION_VIEWS_MAPPER",
            "            ");
        private void AddIDToViewMapper(bool refresh, string filePath, string newIDName, string tag, string space)
        {
            bool error = !TryGetTextMatchCountAboveTag(tag, filePath,"new ViewConfigData", out int configsCount);

            if (error)
            {
                Debug.LogError("Error here");
                return;
            }

            AddTextAboveTag(tag, filePath, space,GetNewMapperLine());
            if (refresh) AssetDatabase.Refresh();

            string[] GetNewMapperLine()
            {
                var id = configsCount + 1;
                var idDescription = "/* " + id +"*/";
                var mainLine = String.Format(@"new ViewConfigData(ViewConfigID.{0:s}, ViewID.{1:s}, typeof({2:s}), typeof({3:s}), LayerID.{4:s}),",
                                viewConfigIDName,viewIDName,viewName,viewControllerName,layerID.ToString());

                return new string[]{idDescription,mainLine};
            }
        }

        private void CreateViewPrefabAndCache()
        {
            
            var newPrefabPath = settings.ViewPrefabsFolderPath + "/" + viewName + ".prefab";
            var newPrefabInstance = Instantiate(settings.VewPrefabTeamplate as GameObject);

            var viewType = GetTypeByName("Game.Views." + viewName);
            newPrefabInstance.AddComponent(viewType);

            var newPrefab = PrefabUtility.CreatePrefab(newPrefabPath,newPrefabInstance as GameObject);
            lastCreatedPrefab = newPrefab;

            GameObject.DestroyImmediate(newPrefabInstance);

            Selection.activeObject = lastCreatedPrefab;
            EditorGUIUtility.PingObject(lastCreatedPrefab);
        }

        private void MapViewPrefabToViewSystem()
        {
            if (lastCreatedPrefab == null)
            {
                if (EditorUtility.DisplayDialog("Error", "lastCreatePrefab == null", "Ok"))
                    return;
            }

            var viewType = GetTypeByName("Game.Views." + viewName);

            var viewComponent = (lastCreatedPrefab as GameObject).GetComponent(viewType);


            bool ok = EditorUtility.DisplayDialog("Внимание", "Сохранять изменения в префабе ViewsSystem?", "Да", "Нет");

            BindComponentInViewSystemProcess(viewComponent,ok);
        }

        private void BindComponentInViewSystemProcess(Component component, bool applyChangesToPrefab = false)
        {
            ViewsProvider viewsProvider = GameObject.FindObjectOfType(typeof(ViewsProvider)) as ViewsProvider;

            if(viewsProvider == null)
            {
                if (EditorUtility.DisplayDialog("Error", "Не удалось найти ViewsSystem prefab на сцене. Откройте Core сцену как главную", "Ok"))
                    return;
            }

            viewsProvider.ExpandAndAddComponent(component);
            EditorUtility.SetDirty(viewsProvider);
            
            if(applyChangesToPrefab)
                PrefabUtility.ApplyPrefabInstance(viewsProvider.gameObject, InteractionMode.AutomatedAction);

            Selection.activeObject = viewsProvider;
            EditorGUIUtility.PingObject(viewsProvider);
        }
    

        string ViewFile()
        {
            return
                "using Core.Views;\n" +
                "using System;\n" +
                "using UnityEngine;\n" +
                "using UnityEngine.UI;\n" +
                "\n" +
                "namespace Game.Views\n" +
                "{\n" +
                k_Tab + "public class " + viewName + " : ViewBase\n" +
                k_Tab + "{\n" +
                "\n" +
                k_Tab + "}\n" +
                "}\n";
        }
        string ViewControllerFile()
        {
            return
                "using Game.Views;\n" +
                "using Core.Controllers;\n" +
                "\n" +
                "namespace Game.Controllers\n" +
                "{\n" +
                k_Tab + "public class " + viewControllerName + " : ViewControllerBase<" + viewName + ">\n" +
                k_Tab + "{\n" +
                k_Tab + k_Tab + "protected override void Show() \n" +
                k_Tab + k_Tab + "{\n" +
                k_Tab + k_Tab + "}\n" +
                "\n" +

                k_Tab + k_Tab + "protected override void Hide() \n" +
                k_Tab + k_Tab + "{\n" +
                k_Tab + k_Tab + "}\n" +
                "\n" +

                k_Tab + "}\n" +
                "}\n";
        }

        

        

        [Serializable]
        public class Settings
        {
            [SerializeField] private UnityEngine.Object viewsFolder;
            [SerializeField] private UnityEngine.Object viewControllersFolder;
            [SerializeField] private UnityEngine.Object viewIDFile;
            [SerializeField] private UnityEngine.Object viewConfigIDFile;
            [SerializeField] private UnityEngine.Object viewsMapper;
            [SerializeField] private UnityEngine.Object viewSystemPrefab;
            [SerializeField] private UnityEngine.Object viewPrefabsFolder;
            [SerializeField] private UnityEngine.Object viewPrefabTeamplate;

            public string ViewsFolderPath => viewsFolder.Path();
            public string ViewControllersFolderPath => viewControllersFolder.Path();
            public string ViewIDFilePath => viewIDFile.Path();
            public string ViewConfigIDFilePath => viewConfigIDFile.Path();
            public string ViewsMapperPath => viewsMapper.Path();
            public string ViewSystemPrefabPath => viewSystemPrefab.Path();
            public string ViewPrefabsFolderPath => viewPrefabsFolder.Path();
            public UnityEngine.Object VewPrefabTeamplate => viewPrefabTeamplate;
            public UnityEngine.Object ViewSystemPrefab => viewSystemPrefab;
        }
    }
}
