using ActionsCollections;
using Game.Views;
using Game.Models;
using UnityEngine;
using System.Linq;
using CustomeEditorTools;
using System.Collections.Generic;

namespace DebugActions
{
    public class ViewActionResetObjectives : ActionBase
    {
        private ObjectivesWindowModel objectivesWindowModel;
        private ObjectivesWindowModel ObjectivesWindowModel
        {
            get
            {
                if(objectivesWindowModel == null)
                {
                    objectivesWindowModel = GameObject.FindObjectOfType<ObjectivesWindowModel>();
                }
                return objectivesWindowModel;
            }
        }

        private string _operationName;

        public ViewActionResetObjectives(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            ObjectivesWindowModel.Reset();
        }
    }
}