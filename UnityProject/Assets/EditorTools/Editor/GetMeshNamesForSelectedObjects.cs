using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace CustomeEditorTools
{
    public static class GetMeshNamesForSelectedObjects
    {
        [MenuItem("Tools/Get mesh names for selected objects")]
        private static void GetMeshNamesForSelectedObjectsMethod()
        {
            var selection = Selection.gameObjects;

            List<string> meshNames = new List<string>();

            foreach (var item in selection)
            {
                var meshCollider = item.GetComponent<MeshCollider>();
                if(meshCollider == null) continue;

                var name = meshCollider.sharedMesh.name;

                if(meshNames.Contains(name)) continue;
                meshNames.Add(name);
            }

            Debug.Log("Mesh count: " + meshNames.Count);
            meshNames.ForEach(x => Debug.Log(x));
        }
    }


}