using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    [DefaultExecutionOrder(-999)]
    public abstract class InjectionMapperBase : MonoBehaviour
    {
        public Dictionary<Type, object> Links { get; protected set; }

        private InjectionSystem InjectionSystem => FindObjectOfType<InjectionSystem>();

        protected virtual void UpdatedLinks()
        {
            
        }

        private void Awake()
        {
            InitLinks();
        }

        protected abstract void InitLinks();

        /* Move out to other place, but currently dont work properly cause of execution order */
        private void OnEnable()
        {
            InjectionSystem?.UpdateLinks(Links);
            UpdatedLinks();
        }

        private void OnDisable()
        {
            InjectionSystem?.RemoveLinks(Links);
        }
    }
}

