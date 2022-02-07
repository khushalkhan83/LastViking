using UnityEditor;
using UnityEditor.SceneManagement;
using Helpers;
using UnityEngine;

namespace CustomeEditorTools
{
    public static class ObjectsMovementTool
    {
        private static GameObject target;

        [MenuItem("EditorTools/LevelDesign/ObjectMovement:(1)Set target")]
        public static void SetSelected()
        {
            target = Selection.activeGameObject;

            if(target == null)
            {
                EditorUtility.DisplayDialog("Error", "Selected is null", "Ok");
                return;
            }
        }
        [MenuItem("EditorTools/LevelDesign/ObjectMovement:(2)Move to selected")]
        public static void SetParrentAndMoveSelectedToTarget()
        {
            var newParrent = Selection.activeGameObject;
            if(newParrent == null)
            {
                EditorUtility.DisplayDialog("Error", "Target parrent is null", "Ok");
                return;
            }
            target.transform.SetParent(newParrent.transform);
        }
    }
}
