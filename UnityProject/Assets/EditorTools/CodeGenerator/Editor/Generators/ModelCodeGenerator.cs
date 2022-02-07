using System;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using Game.Models;
using NaughtyAttributes;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace CustomeEditorTools.CodeGenerator
{
    // [CreateAssetMenu(fileName = "ModelCodeGenerator", menuName = "GeneratorData/SO_Generator_Model", order = 0)]
    public class ModelCodeGenerator : CodeGeneratorBase
    {
        [SerializeField] private string modelName;
        [SerializeField] private string newHolderName;

        [SerializeField] private Settings settings;
        private CodeGeneratorWindowData.GlobalSettings globalSettings => CodeGeneratorWindowData.Instance.globalSettings;

        [Button] void STEP_1_CreateScript() { CreateModelScript(true); CacheCreatedFiles(); }
        [Button] void STEP_2_AddFieldToInject() => AddFieldToInject(true);
        [Button] void STEP_3_AddModelToCoreAndBindInInjectionSystem() => AddToCoreAndInjectionSystem(true);
        [Button] void STEP_4_AddFieldToModels() => AddFieldToModels(true);
        [Button] void STEP_5_AddToModelsSystem() => BindInModelsSystem(true);

        private void AddToCoreAndInjectionSystem(bool showWarningMessage)
        {
            if (showWarningMessage)
            {
                bool cancel = !EditorUtility.DisplayDialog("Внимание", "Префаб Models и Core сцена будут изменены. Продолжить?", "Да", "Нет");
                if (cancel) return;
            }

            AddModelToCoreAndBindField(modelName, true, newHolderName);
        }

        private void AddFieldToModels(bool refresh)
        {
            var type = GetTypeByName(GetAssemblyModelName(modelName));
            AddFieldToModelsSystem(type,refresh);
        }

        private void BindInModelsSystem(bool showWarningMessage)
        {
            if (showWarningMessage)
            {
                bool cancel = !EditorUtility.DisplayDialog("Внимание", "Префаб Models будет изменен автоматически. Сцена будет помечана как измененная. Продолжить?", "Да", "Нет");
                if (cancel) return;
            }

            var type = GetTypeByName(GetAssemblyModelName(modelName));
            BindFieldToModelsSystem(type, true);
        }


        private void AddFieldToModelsSystem(Type type, bool refresh)
        {
            // ser field text
            string typeName = type.Name;
            string typeNameExeptFirstLatter = typeName;
            typeNameExeptFirstLatter = typeNameExeptFirstLatter.Remove(0, 1);
            string typeNameModif = "_" + typeName.First().ToString().ToLower() + typeNameExeptFirstLatter;


            AddTextAboveTag("///CODE_GENERATION_MODELS_SYSTEM", settings.ModelsSystemFilePath, FiledText(), "        ");
            if (refresh) AssetDatabase.Refresh();

            string FiledText() => "public " + typeName + " " + typeNameModif + ";";
        }

        private void BindFieldToModelsSystem(Type type, bool applyChangesToModelsPrefab)
        {
            var coreScene = SceneManager.GetActiveScene();

            if (coreScene.name != "CoreScene")
            {
                if (EditorUtility.DisplayDialog("Error", "Откройте CoreScene (должна быть выделена как активная)", "ok"))
                    return;
            }

            bool error = !TryGetModelsGameObject(out GameObject modelsGameObject);
            if (error)
            {
                if (EditorUtility.DisplayDialog("Error", "Не удалось найти Models на сцене. Откройте CoreScene (должна быть выделена как активная)", "ok"))
                    return;
            }

            var modelsSystem = modelsGameObject.GetComponentInChildren<ModelsSystem>();

            if(modelsSystem == null)
            {
                if (EditorUtility.DisplayDialog("Error", "Не удалось найти ModelsSystem на сцене. Откройте CoreScene (должна быть выделена как активная)", "ok"))
                    return;
            }

            var targetValue = modelsGameObject.GetComponentInChildren(type);

            if(targetValue == null)
            {
                if (EditorUtility.DisplayDialog("Error", "Не удалось найти компонет " + type.ToString() + " в Models. Убедитесь что модель данного типа добавлена в префаб Models", "ok"))
                    return;
            }

            SetSerializedFieldInModelsSystem(type,targetValue);
            if (applyChangesToModelsPrefab)
                    PrefabUtility.ApplyPrefabInstance(modelsGameObject.gameObject, InteractionMode.AutomatedAction);
        }

        private void SetSerializedFieldInModelsSystem(Type type, System.Object valueToSet)
        {
            ModelsSystem modelsSystem = GameObject.FindObjectOfType(typeof(ModelsSystem)) as ModelsSystem;
            modelsSystem.SetLinkValue(type, valueToSet);
        }

        string ModelPath => FullPath(settings.ModelsFolderPath, modelName);

        private void CreateModelScript(bool refresh)
        {
            var dataToCheck = new CodeGeneratorBase.FilePathData[]{
                new CodeGeneratorBase.FilePathData(ModelPath,true),
            };

            bool someFilesExist = CheckForFileExistErrors(dataToCheck);

            if (someFilesExist)
            {
                throw new System.Exception("Some files exist");
            }

            CreateScript(ModelPath, ModelFile());

            if (refresh)
                AssetDatabase.Refresh();
        }

        private void AddFieldToInject(bool refresh)
        {
            GenerateFieldsInInjectionSystem(GetAssemblyModelName(modelName), refresh);
        }

        private void CacheCreatedFiles()
        {
            CacheCreatedFile(ModelPath);
            AssetDatabase.SaveAssets();
        }

        private string ModelFile()
        {
            return
                "using System;\n" +
                "using UnityEngine;\n" +
                "\n" +
                "namespace Game.Models\n" +
                "{\n" +
                k_Tab + "public class " + modelName + " : MonoBehaviour\n" +
                k_Tab + "{\n" +
                "\n" +
                k_Tab + "}\n" +
                "}\n";
        }


        [System.Serializable]
        public class Settings
        {
            [SerializeField] private UnityEngine.Object modelsFolder;
            [SerializeField] private UnityEngine.Object modelsSystemFile;

            public string ModelsFolderPath => modelsFolder.Path();
            public string ModelsSystemFilePath => modelsSystemFile.Path();
        }
    }
}