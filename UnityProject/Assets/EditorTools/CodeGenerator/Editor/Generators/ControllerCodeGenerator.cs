using UnityEngine;
using Extensions;
using NaughtyAttributes;
using UnityEditor;

namespace CustomeEditorTools.CodeGenerator
{
    // [CreateAssetMenu(fileName = "ControllerCodeGenerator", menuName = "GeneratorData/SO_Generator_Controller", order = 0)]
    public class ControllerCodeGenerator : CodeGeneratorBase
    {
        [SerializeField] private string controllerName;
        [SerializeField] private string controllerFileFolderPath;
        [SerializeField] private bool createController;
        [SerializeField] private string interfaceName;
        [SerializeField] private bool createControllerInterface;
        [SerializeField] private Settings settings;

        [Button] void SetDestination()
        {
            // var rootFolder = Application.dataPath.Remove("Assets")
            var settingsPath = settings.ControllersFolderPath;
            var result = EditorUtility.SaveFilePanel("Select controller path", settingsPath, $"{controllerName}.cs", "cs");

            var formatedPath = result.Replace(Application.dataPath,"Assets");

            var fileNameStartIndex = formatedPath.LastIndexOf("/") + 1;
            var fileName = formatedPath.Substring(fileNameStartIndex).Replace(".cs",string.Empty);

            controllerName = fileName;
            interfaceName = $"I{fileName}";

            controllerFileFolderPath = formatedPath.Substring(0,fileNameStartIndex - 1);
        }

        [Button]
        void STEPS_All()
        {
            CreateControllerAndInterface(false);
            GenerateControllerID(false);
            AddControllerIDToControllersSystem(false);

            AssetDatabase.Refresh();
            CacheCreatedFiles();
        }
        [Button] void STEP_1_CreateControllerAndInterface() { CreateControllerAndInterface(true); CacheCreatedFiles(); }
        [Button] void STEP_2_GenerateControllerID() => GenerateControllerID(true);
        [Button] void STEP_3_AddControllerIDToControllersSystem() => AddControllerIDToControllersSystem(true);

        private string ControllerPath => FullPath(controllerFileFolderPath, controllerName);
        private string ControllerInterfacePath => FullPath(settings.ControllersInterfacesFolderPath, interfaceName);

        private void CreateControllerAndInterface(bool refresh)
        {

            var dataToCheck = new CodeGeneratorBase.FilePathData[]{
            new CodeGeneratorBase.FilePathData(ControllerPath,createController),
            new CodeGeneratorBase.FilePathData(ControllerInterfacePath,createControllerInterface),
        };

            bool someFilesExist = CheckForFileExistErrors(dataToCheck);

            if (someFilesExist)
            {
                throw new System.Exception("Some files exist");
            }

            bool pathInRootFolder = ControllerPath.Contains(settings.ControllersFolderPath);
            if(!pathInRootFolder)
            {
                throw new System.Exception("Wrong path. Use SetDestination");
            }

            if (createController) CreateScript(ControllerPath, ControllerFile());
            if (createControllerInterface) CreateScript(ControllerInterfacePath, ControllerInterfaceFile());

            if (refresh) AssetDatabase.Refresh();
        }

        private void CacheCreatedFiles()
        {
            if (createController) CacheCreatedFile(ControllerPath);
            if (createControllerInterface) CacheCreatedFile(ControllerInterfacePath);

            AssetDatabase.SaveAssets();
        }


        private void GenerateControllerID(bool refresh)
        {
            var path = settings.ControllerIDFilePath;
            bool error = !TryGetClosestNumInLineAboveTag("///CODE_GENERATION_IDS", path, out int lastIDNum);

            if (error)
            {
                Debug.LogError("Error here");
                return;
            }

            AddTextAboveTag("///CODE_GENERATION_IDS", path, GetIDLine(), "        ");
            if (refresh) AssetDatabase.Refresh();

            string GetIDLine()
            {
                var controllerIDName = this.controllerName;
                var id = lastIDNum + 1;
                var answer = controllerIDName + " = " + id + ",";
                return answer;
            }
        }

        private void AddControllerIDToControllersSystem(bool refresh)
        {
            var path = settings.ControllersSystemFilePath;
            AddTextAboveTag("///CODE_GENERATION_IDS", path, GetIDLine(), "            ");
            AddTextAboveTag("///CODE_GENERATION_INTERFACES", path, GetInterfaceLine(), "            ");
            if (refresh) AssetDatabase.Refresh();

            string GetIDLine()
            {
                var controllerIDName = this.controllerName;
                var interfaceName = this.interfaceName;
                var answer = "{ ControllerID." + controllerIDName + ", typeof(" + interfaceName + ") },";
                return answer;
            }

            string GetInterfaceLine()
            {
                var controllerTypeName = this.controllerName;
                var interfaceName = this.interfaceName;
                var answer = "{ typeof(" + interfaceName + "), typeof(" + controllerTypeName + ") },";
                return answer;
            }
        }

        private string ControllerFile()
        {
            return
                "using Core;\n" +
                "using Core.Controllers;\n" +
                "using Game.Models;\n" +
                "\n" +
                "namespace Game.Controllers\n" +
                "{\n" +
                k_Tab + "public class " + controllerName + " : I" + controllerName + ", IController\n" +
                k_Tab + "{\n" +
                k_Tab + k_Tab + "void IController.Enable() \n" +
                k_Tab + k_Tab + "{\n" +
                k_Tab + k_Tab + "}\n" +
                "\n" +

                k_Tab + k_Tab + "void IController.Start() \n" +
                k_Tab + k_Tab + "{\n" +
                k_Tab + k_Tab + "}\n" +
                "\n" +

                k_Tab + k_Tab + "void IController.Disable() \n" +
                k_Tab + k_Tab + "{\n" +
                k_Tab + k_Tab + "}\n" +
                "\n" +

                k_Tab + "}\n" +
                "}\n";
        }

        private string ControllerInterfaceFile()
        {
            return
                "using System;\n" +
                "using UnityEngine;\n" +
                "\n" +
                "namespace Game.Controllers\n" +
                "{\n" +
                k_Tab + "public interface " + interfaceName + "\n" +
                k_Tab + "{\n" +
                "\n" +
                k_Tab + "}\n" +
                "}\n";
        }

        [System.Serializable]
        public class Settings
        {
            [SerializeField] private UnityEngine.Object controllersFolder;
            [SerializeField] private UnityEngine.Object controllersInterfacesFolderPath;
            [SerializeField] private UnityEngine.Object controllerIDFile;
            [SerializeField] private UnityEngine.Object controllersSystemFile;

            public string ControllersFolderPath => controllersFolder.Path();
            public string ControllersInterfacesFolderPath => controllersInterfacesFolderPath.Path();
            public string ControllerIDFilePath => controllerIDFile.Path();
            public string ControllersSystemFilePath => controllersSystemFile.Path();
        }
    }
}