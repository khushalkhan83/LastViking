
using UnityEditor;
using UnityEngine;

namespace CustomeEditorTools
{
    /// Prototype.
    public static class LODsTools
    {
        [MenuItem("Tools/LODs/Set Lods")]
        static void SetLods()
        {
            var selectedObject = Selection.activeGameObject;

            if (selectedObject == null)
            {
                EditorUtility.DisplayDialog("Error", "Nothing selected", "Ok");
                return;
            }

            var lodGroup = selectedObject.GetComponent<LODGroup>();

            if (lodGroup == null)
            {
                EditorUtility.DisplayDialog("Error", "No LODGroup component on this object", "Ok");
                return;
            }

            var lods = lodGroup.GetLODs();

            if (lods.Length != 3)
            {
                EditorUtility.DisplayDialog("Error", "This tool limited for 3 lods only", "Ok");
                return;
            }


            lods[0].screenRelativeTransitionHeight = 0.8f;
            lods[1].screenRelativeTransitionHeight = 0.6f;
            lods[2].screenRelativeTransitionHeight = 0.04f;

            Undo.RegisterCompleteObjectUndo(lodGroup, "Object lods change");

            lodGroup.SetLODs(lods);
        }
    }
}