using UnityEngine;
using UnityEditor;
using Extensions;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

namespace CustomeEditorTools
{
    /// <summary>
    /// Used to open spesific prefab in prefab mode and select his child gameObject (TODO: add: display individual component)
    /// </summary>
    public class PrefabDisplayWindow : EditorWindow
    {
        [MenuItem("Assets/Open In Prefab Window")]
        private static void OpenMenuClick(MenuCommand command)
        {
            ShowWindow();
            var window = GetWindow<PrefabDisplayWindow>();
            window.prefab = Selection.activeObject as GameObject;

        }
         // Note that we pass the same path, and also pass "true" to the second argument.
        [MenuItem("Assets/Open In Prefab Window", true)]
        private static bool OpenMenuClickValidation()
        {
            // This returns true when the selected object is a Variable (the menu item will be disabled otherwise).
            var t = PrefabUtility.GetPrefabAssetType(Selection.activeObject);
            return t != PrefabAssetType.NotAPrefab;
        }

        [MenuItem("EditorTools/Windows/Utils/PrefabDisplay")]
        private static void ShowWindow()
        {
            var window = GetWindow<PrefabDisplayWindow>();
            window.titleContent = new GUIContent("PrefabDisplay");
            window.Show();
        }

        private GameObject prefab;
        private string targetName;

        private void OnGUI()
        {
            prefab = EditorGUILayout.ObjectField("prefab",prefab,typeof(GameObject)) as GameObject;
            targetName = GUILayout.TextField(targetName);

            
            if(GUILayout.Button("SelectComponent")) SelectComponent();
        }

        private void SelectComponent()
        {
            PrefabUtility.LoadPrefabContentsIntoPreviewScene(prefab.Path(),EditorSceneManager.NewPreviewScene());
            AssetDatabase.OpenAsset(prefab);

            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            var root = prefabStage.prefabContentsRoot;

            var allObjects = GameObjectsUtil.GetAllChildsAndParrent(root);

            foreach (var obj in allObjects)
            {
                if(obj.name == targetName)
                {
                    Selection.activeGameObject = obj;
                    break;
                }
            }
        }
    }
}