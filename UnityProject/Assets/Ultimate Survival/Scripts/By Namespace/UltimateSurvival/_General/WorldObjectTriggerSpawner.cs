using Core;
using Core.Storage;
using Game.AI;
using Game.AI.Behaviours.Zombie;
using Game.Models;
using Game.StateMachine.Parametrs;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Extensions;
using Game.Progressables;

namespace Game.Controllers
{
    public class WorldObjectTriggerSpawner : ObjectsTriggerSpawner<GameObject>
    {
        #region Data
#pragma warning disable 0649

        [Header("Settings")]
        [SerializeField] private WorldObjectID _worldObjectId;
        [SerializeField] private string _id;

#pragma warning restore 0649
        #endregion

        public WorldObjectCreator WorldObjectCreator => ModelsSystem.Instance._worldObjectCreator;

        public WorldObjectID WorldObjectID => _worldObjectId;
        public string Id => _id;
        
        protected override GameObject CreateObject() => WorldObjectCreator.CreateAsSpawnable(WorldObjectID, Container.transform.position, Container.transform.rotation, transform.localScale, DataProcessing, transform).gameObject;

        protected override void OnCreate()
        {
            ObjectInstance.transform.parent = Container;
            var links = ObjectInstance.GetComponent<InitializationLinks>();
            if (links) 
                links.TargetBase.SetTarget(Container);
            ObjectInstance.GetComponent<WorldObjectModel>().OnDelete += OnDeleteInstanceHandler;

            if (ProgressStatus == ProgressStatus.InProgress)
            {
                ActivateObject();
            }
            else
            {
                DeactivateObject();
            }
        }

        private void OnDeleteInstanceHandler()
        {
            ObjectInstance.GetComponent<WorldObjectModel>().OnDelete -= OnDeleteInstanceHandler;

            OnDelete();
        }

        private void DataProcessing(IEnumerable<IUnique> uniques)
        {
            foreach (var item in uniques)
            {
                item.UUID = item.UUID + '.' + Id;
            }
        }

        protected override void ActivateObject()
        {
            ObjectInstance.TrySetActive(true);
        }

        protected override void DeactivateObject()
        {
            ObjectInstance.TrySetActive(false);
        }

        public override void ClearProgress()
        {
            if(ObjectInstance != null)
            {
                var worldObjectModel = ObjectInstance.GetComponent<WorldObjectModel>();
                if(worldObjectModel != null)
                {
                    worldObjectModel.Delete();
                }
            }
        }
    }
}
