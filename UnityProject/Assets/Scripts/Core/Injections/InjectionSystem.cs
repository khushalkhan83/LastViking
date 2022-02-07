using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Extensions;
using UnityEngine;

namespace Core
{
    public class InjectionSystem : MonoBehaviour
    {
        public event Action<PropertyInfo, object> OnPropertyInjected;
        public event Action<Dictionary<Type, object>> OnLinksUpdated;

        private Dictionary<Type, object> Links { get; set; } = new Dictionary<Type, object>();

        public void UpdateLinks(Dictionary<Type, object> links)
        {
            foreach (var link in links)
            {
                UpdateLink(link);
            }
            OnLinksUpdated?.Invoke(links);
        }

        public void RemoveLinks(Dictionary<Type, object> links)
        {
            foreach (var link in links)
            {
                RemoveLink(link);
            }
        }

        private void UpdateLink(KeyValuePair<Type, object> link)
        {
            Links[link.Key] = link.Value;
        }

        private void RemoveLink(KeyValuePair<Type, object> link)
        {
            Links.Remove(link.Key);
        }

        public static Type InjectAttribute { get; } = typeof(InjectAttribute);
        public static BindingFlags InjectBindingFlags { get; } = BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.GetProperty | BindingFlags.Instance;

        public void Inject(object obj) => Inject(obj, obj.GetType());

        public void Inject(object obj, Type type)
        {
            var properties = GetPropertiesFromType(type);
            
            foreach (var property in properties)
            {
                if (property.IsDefined(InjectAttribute, true))
                {
                    if(!property.CanWrite) continue;
                    
                    property.SetValue(obj, GetLink(property.PropertyType));
                    OnPropertyInjected(property, obj);
                }
            }
        }

        private List<PropertyInfo> GetPropertiesFromType(Type type)
        {
            List<PropertyInfo> answer = new List<PropertyInfo>();
            answer.AddRange(GetPropertiesByType(type));

            var parrentTypes = type.GetParentTypes();

            foreach (var t in parrentTypes)
            {
                answer.AddRange(GetPropertiesByType(t));
            }

            return answer;
        }

        private static List<PropertyInfo> GetPropertiesByType(Type type)
        {
            return type.GetProperties(InjectBindingFlags).ToList();
        }

        // actually should be private 
        internal object GetLink(Type type)
        {
            if (!Links.TryGetValue(type, out var link))
            {
                link = FindObjectOfType(type);
                Links[type] = link;
                #if UNITY_EDITOR
                type.Error("not mapped in injection. Find used");
                #endif
            }

            return link;
        }
    }
}
