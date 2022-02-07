using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Core
{
    public class InjectionUpdater : MonoBehaviour
    {
        private struct InjectInfo
        {
            public PropertyInfo property;
            public object target;
        }

        [SerializeField] private InjectionSystem injectionSystem;

        private List<InjectInfo> ReInjectables { get; } = new List<InjectInfo>();

        private void OnEnable()
        {
            injectionSystem.OnLinksUpdated += OnLinksUpdated;
            injectionSystem.OnPropertyInjected += OnPropertyInjected;
        }

        private void OnDisable()
        {
            ReInjectables.Clear();
            injectionSystem.OnLinksUpdated -= OnLinksUpdated;
            injectionSystem.OnPropertyInjected -= OnPropertyInjected;
        }

        private void OnLinksUpdated(Dictionary<Type, object> links)
        {
            foreach (var inj in ReInjectables)
            {
                bool typeUpdated = links.ContainsKey(inj.property.PropertyType);
                if (!typeUpdated) continue;
                UpdateType(inj);
            }
        }
        private void OnPropertyInjected(PropertyInfo property, object target)
        {
            if ((property.GetCustomAttribute(InjectionSystem.InjectAttribute, true) as InjectAttribute).IsReinjectable)
            {
                ReInjectables.Add(new InjectInfo { property = property, target = target });
            }
        }


        private void UpdateType(InjectInfo inj)
        {
            object value = injectionSystem.GetLink(inj.property.PropertyType);
            inj.property.SetValue(inj.target, value);
        }
    }
}
