
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CustomeEditorTools
{
    /// Prototype.
    public static class ReplacePrefabTool
    {
        [MenuItem("Tools/Replacement/Replace prefab with lod 2")]
        static void SetLods()
        {
            var selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Nothing selected", "Ok");
                return;
            }
            
            var lod2 = GameObjectsUtil.GetObjectsByFilter(selectedObject,new List<string>(){"lod02"}).FirstOrDefault();
            
            if(lod2 == null)
            {
                lod2 = GameObjectsUtil.GetObjectsByFilter(selectedObject,new List<string>(){"LOD2"}).FirstOrDefault();
            }

            if (lod2 == null)
            {
                EditorUtility.DisplayDialog("Error", "No LOD2 detected", "Ok");
                return;
            }

            var lod2Copy = GameObject.Instantiate(lod2,lod2.transform.position,lod2.transform.rotation);
            lod2Copy.gameObject.name.Replace("(Clone)",string.Empty);
            lod2Copy.transform.SetParent(selectedObject.transform.parent);
            lod2Copy.transform.SetSiblingIndex(selectedObject.transform.GetSiblingIndex());
            lod2Copy.transform.localScale = selectedObject.transform.localScale;

            Undo.RegisterCreatedObjectUndo(lod2Copy,"replacement copy create");
            Undo.DestroyObjectImmediate(selectedObject);

            Selection.activeGameObject = lod2Copy;
        }
    }
}