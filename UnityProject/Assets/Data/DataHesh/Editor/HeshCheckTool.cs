using Core.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HeshCheckTool : EditorWindow
{
    #region Data
#pragma warning disable 0649

#pragma warning restore 0649
    #endregion

    public List<string> uuidObject;

    public Dictionary<string, string> objectsWithData;
    public Dictionary<string, string> objectWithRepeatData;

    public List<string> ObjectInfo;
    public string StringToFind;
    int index = 0;

    [MenuItem("Tools/Hesh Check")]
    static public void ShowWindow()
    {
        GetWindow<HeshCheckTool>();
    }

    Type DataBaseType = typeof(DataBase);
    Type SerializeFieldType = typeof(SerializeField);

    public void ButtonCallback()
    {
        var GameObjects = FindObjectsOfType<GameObject>();

        foreach (var gameObject in GameObjects)
        {
            var components = gameObject.GetComponents<Component>();

            foreach (var component in components)
            {
                var type = component.GetType();
                var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Union(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance).Where(x => x.IsDefined(SerializeFieldType, true)));

                foreach (var field in fields)
                {
                    if (DataBaseType.IsAssignableFrom(field.GetType()))
                    {
                        var data = (DataBase)field.GetValue(component);
                        var uuid = data.UUID;

                        if (objectsWithData.ContainsValue(uuid))
                        {
                            objectWithRepeatData.Add(gameObject.name, uuid);
                            objectWithRepeatData.Add(objectsWithData.FirstOrDefault(x => x.Value == uuid).Key, uuid);

                            ObjectInfo.Add(component.name + "/" + gameObject.name + "/" + uuid);
                            ObjectInfo.Add(component.name + "/" + objectsWithData.FirstOrDefault(x => x.Value == uuid).Key + "/" + uuid);
                        }
                        else
                        {
                            objectsWithData.Add(gameObject.name, uuid);
                        }
                    }
                }
            }
        }
    }

    private void FindObjectInScene()
    {
        if (StringToFind != "")
        {
            var gameObjectName = StringToFind;
            var objectToFind = GameObject.Find(gameObjectName);
            Selection.activeGameObject = objectToFind;
        }
    }

    private void OnGUI()
    {
        GUILayout.BeginVertical();
        if (GUILayout.Button("Get hesh", GUILayout.Width(100)))
        {
            ButtonCallback();
        }
        if (ObjectInfo != null && ObjectInfo.Count > 0)
        {
            for (int i = 0; i < ObjectInfo.Count; i++)
            {
                GUILayout.TextArea(ObjectInfo[i]);
            }
        }   
        GUILayout.EndVertical();

        StringToFind = GUILayout.TextField(StringToFind);
        if (GUILayout.Button("Find Object In Scene", GUILayout.Width(150)))
        {
            FindObjectInScene();
        }
    }

    private void OnDisable()
    {
        if (ObjectInfo != null)
        {
            ObjectInfo.Clear();
        }
        if (objectsWithData != null)
        {
            objectsWithData.Clear();
        }
        if (objectWithRepeatData != null)
        {
            objectWithRepeatData.Clear();
        }
    }
}
