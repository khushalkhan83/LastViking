using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools
{
    public static class EditEditorGameSettings
    {
        [MenuItem("EditorTools/Select Editor Game Settings #%h")] // shift ctrl-h on Windows, shift cmd-h on macOS
        private static void SelectEditorGameSettings()
        {
            EditorGameSettingsWindow.OpenWindow();
            // Selection.activeObject = EditorGameSettings.Instance;
        }
    }
}

