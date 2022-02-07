using UnityEngine;
using UnityEditor;
using Extensions.Editor;
using Extensions;

namespace CustomeEditorTools.CodeGenerator
{
    public class CodeGeneratorWindow : EditorWindow
    {

        private readonly string k_baseClassData = "codeGenerators";
        private SerializedProperty p_baseClassData;
        private SerializedProperty p_element_Selected;

        [MenuItem("EditorTools/Windows/CodeGeneratorWindow_2.0")]
        private static void ShowWindow()
        {
            var window = GetWindow<CodeGeneratorWindow>();
            window.titleContent = new GUIContent("CodeGeneratorWindow_2.0");
            window.Show();
        }

        private static CodeGeneratorWindowData Data => CodeGeneratorWindowData.Instance;

        private SerializedObject serializedObject;

        private void OnEnable()
        {
            serializedObject = null;
            p_baseClassData = null;
            p_element_Selected = null;

            serializedObject = new SerializedObject(Data);
            p_baseClassData = serializedObject.FindProperty(k_baseClassData);
        }

        private void OnGUI()
        {
            if (serializedObject == null) return;
            if (p_baseClassData == null) return;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();

            GUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.MaxWidth(Data.toolBarMaxWidth));
            ToolBar();
            GUILayout.EndHorizontal();

            // EditorGUILayout.PropertyField(p_baseClassData, true);

            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.MaxWidth(Data.sideBarMaxWidth));
            SidePanel();
            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            SelectedGeneratorView();
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();

            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void SidePanel()
        {
            for (int i = 0; i < p_baseClassData.arraySize; i++)
            {
                SerializedProperty p_element = p_baseClassData.GetArrayElementAtIndex(i);
                if (p_element == null) continue;

                if (GUILayout.Button(p_element.objectReferenceValue.name))
                {
                    p_element_Selected = p_element;
                    Data.LastSelectedElementIndex = i;
                }
            }
        }

        private void ToolBar()
        {
            if (GUILayout.Button("Settings", EditorStyles.toolbarButton, GUILayout.MaxWidth(Data.maxToolBarButtonWidth))) Settings();
            if (GUILayout.Button("Help", EditorStyles.toolbarButton, GUILayout.MaxWidth(Data.maxToolBarButtonWidth))) Help();


            void Settings()
            {
                Selection.activeObject = Data;
                EditorGUIUtility.PingObject(Data);
            }
            void Help()
            {
                Application.OpenURL(Data.documentationURL);
            }
        }

        private void SelectedGeneratorView()
        {
            var index = Data.LastSelectedElementIndex;
            if (index == -1) return;

            if (p_element_Selected == null)
            {
                SerializedProperty p_element = p_baseClassData.GetArrayElementAtIndex(index);
                if (p_element == null)
                {
                    Data.LastSelectedElementIndex = -1;
                    return;
                }

                p_element_Selected = p_element;
                Data.LastSelectedElementIndex = index;
            }

            p_element_Selected.AbstractPropertyDrawer();
        }
    }
}