using UnityEngine;
using UnityEditor;
using System;
using UnityEditor.SceneManagement;
using Game.Models;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

namespace CustomeEditorTools
{
    public class ComponentDisplayWindow : EditorWindow {

        // Add a menu item called "Double Mass" to a Rigidbody's context menu.
        [MenuItem("CONTEXT/Component/Display at Component Window")]
        static void SelectInWindow(MenuCommand command)
        {
            ShowWindow();
            var window = GetWindow<ComponentDisplayWindow>();
            
            var gameObject = ((Component)command.context).gameObject;

            var gameObjectPath = GetPath(gameObject);

            window.path = gameObjectPath;
            window.componentName = command.context.GetType().Name;

            window.Refresh();
        }

        private static string GetPath(GameObject go, string path = "")
        {
            if(path == "") path = go.name;
            var p = go.transform.parent;
            if(p == null)
                return path;
            
            path = p.name + "/" + path;
            return GetPath(p.gameObject, path);
        }


        private Editor editor;
        private Vector2 scrollPos;
        private string path;
        private string componentName;

        private bool selectOnRefresh;

        private void OnEnable() {
            SceneManager.sceneLoaded += OnSceneLoaded;
            EditorApplication.playModeStateChanged += PlayModeStateChanged;
        }

        private void OnDisable() {
            SceneManager.sceneLoaded -= OnSceneLoaded;
            EditorApplication.playModeStateChanged -= PlayModeStateChanged;
        }

        private void PlayModeStateChanged(PlayModeStateChange obj)
        {
            Refresh();
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            Refresh();
        }

        private void Refresh()
        {
            DrawEditor();
            if(selectOnRefresh)
                Select();

            Repaint();
        }

        [MenuItem("EditorTools/Windows/Utils/ComponentDisplay")]
        private static void ShowWindow() 
        {
            var window = GetWindow<ComponentDisplayWindow>();
            window.titleContent = new GUIContent("ComponentDisplay");
            window.Show();
        }

        private void OnGUI() 
        {
            path = GUILayout.TextField(path);
            componentName = GUILayout.TextField(componentName);

            if(GUILayout.Button("DrawEditor")) DrawEditor();
            if(GUILayout.Button("Select")) Select();

            selectOnRefresh = EditorGUILayout.Toggle("selectOnRefresh", selectOnRefresh);

            try
            {
                if(editor == null) return;
                if(editor.serializedObject == null)
                {
                    editor = null;
                    return;
                }
            }
            catch (System.Exception)
            {
                return;
            }

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                try
                {
                    editor.OnInspectorGUI();
                }
                catch (System.Exception)
                {
                    
                }
            EditorGUILayout.EndScrollView();
        }


        private void DrawEditor()
        {
            var go = GameObject.Find(path);
            if(go == null)
            {
                editor = null;
                Debug.LogError("Component Display can`t find component. Nothing to Show");
                return;
            }
            
            var component = go.GetComponents(typeof(Component)).ToList().Find(x => x.GetType().Name == componentName);

            editor = UnityEditor.Editor.CreateEditor(component);

        }

        private void Select()
        {
            var go = GameObject.Find(path);
            Selection.activeGameObject = go;
        }
    }
}