using UnityEditor;
using UnityEngine;

namespace CustomEditorTools
{
    public static class SnapToGroundTool
    {
        [MenuItem("EditorTools/LevelDesign/Snap To Ground %h")]
        public static void Ground()
        {
            foreach(var transform in Selection.transforms)
            {
                var hits = Physics.RaycastAll(transform.position + Vector3.up, Vector3.down, 500f);
                foreach(var hit in hits)
                {
                    if (hit.collider.gameObject == transform.gameObject)
                        continue;

                    transform.position = hit.point;
                    break;
                }
            }
        }
    }
}