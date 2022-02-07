using System;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using PWCommon1;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using Core;
using System.Linq;
using System.Collections.Generic;

namespace CustomeEditorTools.CodeGenerator
{
    using static CustomeEditorTools.CodeGenerator.CodeGeneratorWindowData;
    public class CodeGeneratorBase : ScriptableObject
    {
        [SerializeField] private List<UnityEngine.Object> lastCreatedFiles;

        private GlobalSettings settings => CodeGeneratorWindowData.Instance.globalSettings;

        protected const string k_Tab = "    ";

        protected string GetAssemblyModelName(string modelName) => "Game.Models." + modelName;
        protected void AddTextAboveTag(string tag, string path, string content, string newLineSpace)
        {
            AddTextAboveTag(tag,path,newLineSpace, new string[] {content});
        }

        protected void AddTextAboveTag(string tag, string path,string newLineSpace, params string[] content)
        {
            string originalFile;

            using (StreamReader reader = File.OpenText(path))
            {
                originalFile = reader.ReadToEnd();
            }

            if (!originalFile.Contains(tag)) new Exception("Cant find TAG");

            using (StreamWriter writer = File.CreateText(path))
            {
                string newContent = string.Empty;
                for (int i = 0; i < content.Length; i++)
                {
                    newContent += content[i] + "\r\n" + newLineSpace;
                }
                newContent+= tag;

                var newFile = originalFile.Replace(tag, newContent);
                writer.Write(newFile);
            }
        }

        protected bool CheckForFileExist(string path)
        {
            bool fileExist = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object)) != null;
            if (fileExist)
            {
                if (EditorUtility.DisplayDialog("Error", "Файл с тиким именем по указанному пути существует." + path, "ok"))
                    return true;
            }
            return false;
        }

        protected bool CheckForFileExistErrors(params FilePathData[] datas)
        {
            bool errors = false;
            for (int i = 0; i < datas.Length; i++)
            {
                var data = datas[i];
                if (!data.check) continue;

                bool error = CheckForFileExist(data.path);
                if (error) errors = true;
            }
            return errors;
        }

        protected void CreateScript(string path, string content)
        {
            using (StreamWriter writer = File.CreateText(path))
                writer.Write(content);
        }

        //FIXME: this method don`t use tag
        protected bool TryGetClosestNumInLineAboveTag(string tag, string path, out int answer)
        {
            answer = 0;
            string text;
            using (StreamReader reader = File.OpenText(path))
            {
                text = reader.ReadToEnd();
            }

            var lines = text.Split('\n');


            for (int i = lines.Length - 1; i >= 0; i--)
            {
                var tempLine = lines[i];
                string[] numers = Regex.Split(tempLine, @"\D+");

                if (numers.Length == 0) continue;

                foreach (var value in numers)
                {
                    if (!string.IsNullOrEmpty(value))
                    {
                        int num = int.Parse(value);
                        answer = num;
                        return true;
                    }
                }
            }

            return false;
        }

        //FIXME: this method don`t use tag
        protected bool TryGetTextMatchCountAboveTag(string tag, string path, string input, out int answer)
        {
            answer = 0;
            string text;
            using (StreamReader reader = File.OpenText(path))
            {
                text = reader.ReadToEnd();
            }

            var lines = text.Split('\n');

            answer = lines.Where(x => x.Contains(input)).Count();
            return true;
        }

        protected string FullPath(string folder, string fileName) => folder + "/" + fileName + ".cs";


        protected void GenerateFieldsInInjectionSystem(string assemblyName, bool refresh = true)
        {
            Type t = Utils.GetType(assemblyName);
            AddFieldToInjectSystem(t, refresh);
        }
        protected void AddModelToCoreAndBindField(string modelName, bool applyChangesToModelsPrefab, string newHolderName)
        {
            Type t = Utils.GetType(GetAssemblyModelName(modelName));
            BindModelProcess(t, applyChangesToModelsPrefab, newHolderName);
        }

        protected Type GetTypeByName(string assemblyName)
        {
            Type t = Utils.GetType(assemblyName);
            return t;
        }

        private void AddFieldToInjectSystem(Type type, bool refresh)
        {
            // ser field text
            string typeName = type.Name;
            string typeNameExeptFirstLatter = typeName;
            typeNameExeptFirstLatter = typeNameExeptFirstLatter.Remove(0, 1);
            string typeNameModif = "_" + typeName.First().ToString().ToLower() + typeNameExeptFirstLatter;


            AddTextAboveTag("///CODE_GENERATION_FIELDS", settings.InjectionSystemFilePath, FiledText(), "        ");
            AddTextAboveTag("///CODE_GENERATION_LINKS", settings.InjectionSystemFilePath, LinkText(), "                ");
            if (refresh) AssetDatabase.Refresh();

            string FiledText() => "[SerializeField] private " + typeName + " " + typeNameModif + ";";
            string LinkText() => "{ typeof(" + typeName + "), " + typeNameModif + "},";
        }

        private void BindModelProcess(Type componentType, bool applyChangesToModelsPrefab, string newHolderName)
        {
            var coreScene = SceneManager.GetActiveScene();
            string holderName = newHolderName == string.Empty ? "NewHolder" : newHolderName;

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

            var modelsGameObjects = CustomeEditorTools.GameObjectsUtil.GetChildsAndParrent(modelsGameObject);

            GenericMenu menu = new GenericMenu();
            foreach (var model in modelsGameObjects)
            {
                menu.AddItem(new GUIContent("Existing/" + model.name), false, AddComponent_Bind, model);
            }
            menu.AddSeparator("");
            menu.AddItem(new GUIContent("New/" + holderName), false, AddNewHolder_AddComponent_Bind);

            #region Button clicks fucntions

            void AddNewHolder_AddComponent_Bind()
            {
                var newModelsHolder = new GameObject();
                newModelsHolder.name = holderName;

                newModelsHolder.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
                newModelsHolder.transform.SetParent(modelsGameObject.transform);
                if (applyChangesToModelsPrefab)
                    PrefabUtility.ApplyAddedGameObject(newModelsHolder, settings.ModelsPrefabPath, InteractionMode.AutomatedAction);

                AddComponent_Bind(newModelsHolder);
            }
            void AddComponent_Bind(object holder)
            {
                var component = (holder as GameObject).AddComponent(componentType);
                PrefabUtility.ApplyAddedComponent(component, settings.ModelsPrefabPath, InteractionMode.AutomatedAction);
                SetSerializedFieldInInject(componentType, component);
                EditorSceneManager.MarkSceneDirty(coreScene);

                bool ok = EditorUtility.DisplayDialog("Внимание", "Core сцена изменена (injection prefab). Хотите сохранить?", "Да", "Нет");

                if (ok) SaveCoreScene();
            }

            void SaveCoreScene()
            {
                EditorSceneManager.SaveScene(coreScene);
            }

            #endregion

            menu.ShowAsContext();
        }

        protected bool TryGetModelsGameObject(out GameObject modelsGameObject)
        {
            modelsGameObject = null;
            var models = SceneManager.GetActiveScene().GetRootGameObjects().ToList().Find(x => x.name == "Models");
            if (models == null) return false;

            modelsGameObject = models;
            return true;
        }

        private void SetSerializedFieldInInject(Type type, System.Object valueToSet)
        {
            CoreInjectionMapper mapper = GameObject.FindObjectOfType(typeof(CoreInjectionMapper)) as CoreInjectionMapper;
            mapper.SetLinkValue(type, valueToSet);
        }

        protected void CacheCreatedFile(string path)
        {
            var newFile = AssetDatabase.LoadAssetAtPath(path, typeof(UnityEngine.Object));
            lastCreatedFiles.Add(newFile);
        }

        public class FilePathData
        {
            public string path;
            public bool check;

            public FilePathData(string path, bool check)
            {
                this.path = path;
                this.check = check;
            }
        }
    }
}