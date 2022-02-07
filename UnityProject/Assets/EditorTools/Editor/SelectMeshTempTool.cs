using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Linq;
using System;
using cakeslice;

namespace CustomeEditorTools
{

    public static class SelectRocsLods
    {
        [MenuItem("Tools/Temp/Select roks mesh")]
        private static void Execute()
        {
            var filteredSelection = GameObjectsUtil.GetObjectsByFilter(Selection.activeGameObject,
                                                                                new List<string>() { "stone", "resource" });

            if(filteredSelection.Count == 0) 
                if(EditorUtility.DisplayDialog("Error", "Файлы по фильтру не найдены", "Ok"))
                    return;

            var meshes = new List<MeshRenderer>();
            filteredSelection.ForEach(x => meshes.AddRange(x.GetComponentsInChildren<MeshRenderer>()));

            var answer = new List<GameObject>();
            meshes.ForEach(x => answer.Add(x.gameObject));
            

            Selection.objects = answer.ToArray();
        }
    }

    public static class SelectRocsWithOutlineAndNullRenderer
    {
        [MenuItem("Tools/Temp/Select roks mesh with null renderers")]
        private static void Execute()
        {
            var filteredSelection = GameObjectsUtil.GetObjectsByFilter(Selection.activeGameObject,
                                                                                new List<string>() { "stone", "resource" });

            if(filteredSelection.Count == 0) 
                if(EditorUtility.DisplayDialog("Error", "Файлы по фильтру не найдены", "Ok"))
                    return;

            var outlines = new List<Outline>();
            filteredSelection.ForEach(x => outlines.AddRange(x.GetComponentsInChildren<Outline>()));

            var answer = new List<GameObject>();
            outlines.Where(x => x.Renderer == null).ToList().ForEach(y => answer.Add(y.gameObject));
            
            Selection.objects = answer.ToArray();
        }

        [MenuItem("Tools/Temp/Get filtered game objects")]
         private static void Execute1()
        {
            var filteredSelection = GameObjectsUtil.GetObjectsByFilter(Selection.activeGameObject,
                                                                                new List<string>() { "resource", "fractured" });

            Selection.objects = filteredSelection.Select(x => x.GetComponentInChildren<MinebleFractureObject>().gameObject).ToArray();
            filteredSelection.Count.Log();
        }

    }
}