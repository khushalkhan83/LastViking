using System;
using System.Collections.Generic;
using System.Linq;
using Core.Views;
using Game.Views;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
#endif
using UnityEngine;
using UnityEngine.UI;

namespace Extensions
{
    public static class Extensions
    {
        public static void TrySetActive(this GameObject go, bool value, bool showErrors = true)
        {
            if (go == null)
            {
                if (showErrors) { Debug.LogError("Null reff here"); }
                return;
            }
            go.SetActive(value);
        }
        public static void TrySetActive(this Initable initable, bool value, bool showErrors = true)
        {
            if (initable == null)
            {
                if (showErrors) { Debug.LogError("Null reff here"); }
                return;
            }
            initable.gameObject.SetActive(value);
        }

        public static GameObject CheckNull(this GameObject go)
        {
            if(go == null) return null;
            else return go;
        }
        public static T CheckNull<T>(this T mono) where T: MonoBehaviour
        {
            if(mono == null) return null;
            else return mono;
        }

        // TODO: make different formats (hh-mm, mm-ss, ...) as enum
        public static string GetFormatedTime(this float time, string format)
        {
            var answer = TimeSpan.FromSeconds(time).ToString(format);
            return answer;
        }

        public static string TimeToString(this float time, string minSecString, string hourMinString)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            if (timeSpan.Hours == 0)
            {
                return string.Format(minSecString, timeSpan.Minutes, timeSpan.Seconds);
            }
            return string.Format(hourMinString, timeSpan.Hours, timeSpan.Minutes);
        }

        #region Arrays

        // The random number generator.
        private static System.Random _rand;
        private static System.Random Rand
        {
            get
            {
                if (_rand == null) _rand = new System.Random();
                return _rand;
            }
        }

        // Return a random item from an array.
        public static T RandomElement<T>(this T[] items)
        {
            // Return a random item.
            return items[Rand.Next(0, items.Length)];
        }

        // Return a random item from a list.
        public static T RandomElement<T>(this List<T> items)
        {
            // Return a random item.
            return items[Rand.Next(0, items.Count)];
        }
        // Return a random item from a ienumerable.
        public static T RandomElement<T>(this IEnumerable<T> items)
        {
            // Return a random item.
            return items.ElementAt(Rand.Next(0, items.Count()));
        }

        public static bool IndexOutOfRange<T>(this List<T> list, int index)
        {
            bool indexOutOfRange = list.Count <= index || index < 0;
            return indexOutOfRange;
        }
        public static bool IndexOutOfRange<T>(this T[] array, int index)
        {
            bool indexOutOfRange = array.Length <= index || index < 0;
            return indexOutOfRange;
        }

        #endregion


        #region Layers and Triggers

        public static bool InsideLayerMask(this GameObject gameObject, LayerMask layerMask)
        {
            bool answer = layerMask == ((1 << gameObject.layer) | layerMask);
            return answer;
        }
        #endregion

        #region  UnityEngine.Object
        
        #if UNITY_EDITOR
        public static string Path(this UnityEngine.Object asset)
        {
            return UnityEditor.AssetDatabase.GetAssetPath(asset);
        }
        #endif

        #region SafeActivateComponent

        public static void SafeActivateComponent<T>(this GameObject gameObject) where T: MonoBehaviour
        {
            bool hasComponent = gameObject.TryGetComponent<T>(out var component);
                if(!hasComponent)
                    gameObject.AddComponent<T>();
                else
                {
                    component.enabled = true;
                }
        }

        public static void SafeDeactivateComponent<T>(this GameObject gameObject) where T: MonoBehaviour
        {
            if(gameObject == null) return;
            
            bool hasComponent = gameObject.TryGetComponent<T>(out var component);
                if(hasComponent)
                    component.enabled = false;
        }

        public static void SafeActivateComponent<T>(this Component component) where T: MonoBehaviour
        {
            component.gameObject.SafeActivateComponent<T>();
        }
        public static void SafeDeactivateComponent<T>(this Component component) where T: MonoBehaviour
        {
            component.gameObject.SafeDeactivateComponent<T>();
        }

        #endregion

        #region Monobehvaiours

        public static bool IsOpenedInPrefabMode(this MonoBehaviour monoBehaviour)
        {
            #if UNITY_EDITOR
            return (PrefabStageUtility.GetPrefabStage(monoBehaviour.gameObject) != null);
            #else
            return false;
            #endif
        }

        public static bool IsEditMode(this MonoBehaviour monoBehaviour)
        {
            #if UNITY_EDITOR
            return !EditorApplication.isPlaying;
            #else
            return false;
            #endif
        }

        #endregion

        #region Canvas
        public static void SetOverrideSorting(this Canvas canvas, bool value)
        {
            canvas.overrideSorting = value;
            canvas.sortingOrder = value ? 100 : 0;

            bool componentExist = canvas.TryGetComponent<GraphicRaycaster>(out var output);
            if(!componentExist) output = canvas.gameObject.AddComponent<GraphicRaycaster>();
        }
        #endregion

        #region ViewSystem

        public static bool TryHideView(this ViewsSystem viewsSystem, IView view)
        {
            if (view == null) return false;

            viewsSystem.Hide(view);
            return true;
        }

        #endregion

        #region Types
            
        public static IEnumerable<Type> GetParentTypes(this Type type)
        {
            // is there any base type?
            if (type == null)
            {
                yield break;
            }

            // return all implemented or inherited interfaces
            foreach (var i in type.GetInterfaces())
            {
                yield return i;
            }

            // return all inherited types
            var currentBaseType = type.BaseType;
            while (currentBaseType != null)
            {
                yield return currentBaseType;
                currentBaseType = currentBaseType.BaseType;
            }
        }

        #endregion

        // Works in C#3/VS2008: 
        // Returns a new dictionary of this ... others merged leftward. 
        // Keeps the type of 'this', which must be default-instantiable. 
        // Example:  
        //   result = map.MergeLeft(other1, other2, ...) 
        public static T MergeLeft<T, K, V>(this T me, params IDictionary<K, V>[] others)
            where T : IDictionary<K, V>, new()
        {
            T newMap = new T();
            foreach (IDictionary<K, V> src in
                (new List<IDictionary<K, V>> { me }).Concat(others))
            {
                // ^-- echk. Not quite there type-system. 
                foreach (KeyValuePair<K, V> p in src)
                {
                    newMap[p.Key] = p.Value;
                }
            }
            return newMap;
        }
        #endregion
    }
}