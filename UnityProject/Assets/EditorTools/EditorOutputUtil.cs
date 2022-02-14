using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CustomeEditorTools
{
    public static class EditorOutputUtil
    {
        public static void PresentMessage(string message)
        {
#if UNITY_EDITOR
            OutputWindow.PresentMessage(message);
#endif
        }
    }

#if UNITY_EDITOR

    public class OutputWindow : EditorWindow
    {
        private static string output;
        private Vector2 scroll;

        // [MenuItem("EditorTools/Testing/OutputWindow")]
        private static void ShowWindow()
        {
            var window = GetWindow<OutputWindow>();
            window.titleContent = new GUIContent("OutputWindow");
            window.Show();
        }

        public static void PresentMessage(string text)
        {
            output = text;
            if (GetWindow<OutputWindow>() == null)
                ShowWindow();
        }

        private void OnGUI()
        {
            if (scroll == null) scroll = new Vector2(0, 0);
            scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.Height(700));
            output = EditorGUILayout.TextArea(output, GUILayout.ExpandHeight(true));
            EditorGUILayout.EndScrollView();
        }
    }
#endif
}