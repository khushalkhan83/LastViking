using Extensions.Editor;
using System.Linq;
using UnityEditor.AnimatedValues;

namespace CustomeEditorTools
{

    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using Game.Objectives.Stacks;

    public class ObjectivesStacksDashboard : EditorWindow
    {
        private ObjectivesWindow oldSelectedData;
        private ObjectivesWindow selectedData;
        private int maxElementPerHorizontal = 7;
        private int maxWidthPerStack = 100;
        private SerializedObject serializedObject;
        private SerializedProperty p_baseClassData;
        private AnimBool _showSettings;
        private bool useStackTypeFilter;
        private bool useObjectivesTypeFilter;
        private bool displayTiers;
        private ObjectivesStack.ObjectivesStackType stackTypeFilter;
        private ObjectivesStack.ObjectivesType objectivesTypeFilter;

        private readonly string k_baseClassData = "staks";
        bool dataChanged;
        private Vector2 scrollPos;

        private void OnEnable() 
        {
            _showSettings = new AnimBool(true);
            _showSettings.valueChanged.AddListener(Repaint);
        }

        [MenuItem("EditorTools/Windows/Objectives/ObjectivesStacksDashboard")]
        private static void ShowWindow()
        {
            var window = GetWindow<ObjectivesStacksDashboard>();
            window.titleContent = new GUIContent("ObjectivesStacksDashboard");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            selectedData = EditorGUILayout.ObjectField(selectedData,typeof(ObjectivesWindow)) as ObjectivesWindow;
            maxElementPerHorizontal = (int)EditorGUILayout.IntField("maxElementPerHorizontal",maxElementPerHorizontal);
            maxWidthPerStack = (int)EditorGUILayout.IntField("maxWidthPerStack",maxWidthPerStack);
            useStackTypeFilter = (bool)EditorGUILayout.Toggle("use",useStackTypeFilter);
            stackTypeFilter = (ObjectivesStack.ObjectivesStackType)EditorGUILayout.EnumPopup("stackTypeFilter:", stackTypeFilter);
            useObjectivesTypeFilter = (bool)EditorGUILayout.Toggle("use",useObjectivesTypeFilter);
            objectivesTypeFilter = (ObjectivesStack.ObjectivesType)EditorGUILayout.EnumPopup("objectivesTypeFilter", objectivesTypeFilter);
            EditorGUILayout.EndHorizontal();
            // if(GUILayout.Button("Test actions")) selectedData?.SelectNewAndCompleteAll();

            dataChanged = selectedData != oldSelectedData;
            oldSelectedData = selectedData;
            if(dataChanged)
            {
                if(selectedData == null)
                {
                    serializedObject = null;
                    p_baseClassData = null;
                }
                else
                {
                    serializedObject = new SerializedObject(selectedData);
                    p_baseClassData = serializedObject.FindProperty(k_baseClassData);
                }
            }

            if(p_baseClassData == null) return;

            serializedObject.Update();
            EditorGUI.BeginChangeCheck();
            // EditorGUILayout.PropertyField(p_baseClassData, true);
            // DisplayWindowSettings();
            DisplayDashBord(maxElementPerHorizontal);
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }

        private void DisplayWindowSettings()
        {
            _showSettings.target = EditorGUILayout.ToggleLeft("ShowSettings", _showSettings.target);

            //Extra block that can be toggled on and off.
            if (EditorGUILayout.BeginFadeGroup(_showSettings.faded))
            {
                if(p_baseClassData == null) return;

                p_baseClassData.AbstractPropertyDrawer();
            }
            EditorGUILayout.EndFadeGroup();
        }

        private void DisplayDashBord(int maxElementPerHorizontal)
        {
            List<SerializedProperty> datas = new List<SerializedProperty>();
            List<DataPerLine> datasPerLines = new List<DataPerLine>();

            var elementsinLine = 0;
            

            #region Form data
                
            
            for (int i = 0; i < p_baseClassData.arraySize; i++)
            {
                elementsinLine++;
                SerializedProperty p_element = p_baseClassData.GetArrayElementAtIndex(i);
                if (p_element == null) continue;

                datas.Add(p_element);
            }

            var index = 0;
            datasPerLines.Add(new DataPerLine());

            foreach (var data in datas)
            {
                if(index >= maxElementPerHorizontal)
                {
                    CreateNewLine();
                    AddDataToLastLine();
                    index = 0;
                }
                else
                {
                    AddDataToLastLine();
                }
                index++;

                void AddDataToLastLine()
                {
                    var currentElement = datasPerLines[datasPerLines.Count - 1];
                    currentElement.props.Add(data);
                }

                void CreateNewLine()
                {
                    var newDataPerLine = new DataPerLine();
                    datasPerLines.Add(newDataPerLine);
                }
            }
            #endregion
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

            // display data per line
            EditorGUILayout.BeginVertical();
            foreach (var dataPerLine in datasPerLines)
            {
                DisplayDataPerLine(dataPerLine);
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.EndScrollView();
        }
        private void DisplayDataPerLine(DataPerLine dataPerLine)
        {
            EditorGUILayout.BeginHorizontal();
           foreach (var prop in dataPerLine.props)
           {
                var stack = prop.objectReferenceValue as ObjectivesStack;
                if(useStackTypeFilter && stackTypeFilter != stack.StackType) continue;
                if(useObjectivesTypeFilter && objectivesTypeFilter != stack.ObjectiveType) continue;

                EditorGUILayout.BeginVertical(GUILayout.MaxWidth(maxWidthPerStack));

                var defaultColor = GUI.color;
                if(stack.IsSelected) GUI.color = Color.green;
                if (GUILayout.Button(prop.objectReferenceValue.name)) {selectedData.RerollStack(stack); }
                if (GUILayout.Button("Complete")) {selectedData.CompleteObjectiveInStack(stack); }
                GUI.color = defaultColor;
                prop.AbstractPropertyDrawer();
                EditorGUILayout.EndVertical();
           }
           EditorGUILayout.EndHorizontal();
        }


        private class DataPerLine
        {
            public List<SerializedProperty> props = new List<SerializedProperty>();
        }
    }

}