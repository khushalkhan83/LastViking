using System;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEngine;

namespace CustomeEditorTools
{
    [InitializeOnLoad]
    public static class PrefabModeTools
    {
        static PrefabModeTools()
        {
            PrefabStage.prefabStageOpened += OnPrefabStageOpened;
            PrefabStage.prefabSaved += OnPrefabSaved;
            PrefabStage.prefabSaving += OnPrefabSaving;
        }

        private static void OnPrefabSaving(GameObject obj)
        {
            var tag = obj.GetComponent<PrefabModePreview>();

            if(tag == null) return;

            obj.SetActive(false);

            // EditorUtility.SetDirty(obj);
        }

        private static void OnPrefabSaved(GameObject obj)
        {
            var tag = obj.GetComponent<PrefabModePreview>();

            if(tag == null) return;

            obj.SetActive(true);
        }

        private static void OnPrefabStageOpened(PrefabStage prefabStage)
        {
            var root = prefabStage.prefabContentsRoot;

            var tag = root.GetComponent<PrefabModePreview>();

            if(tag == null) return;

            root.SetActive(true);
        }
    }
}