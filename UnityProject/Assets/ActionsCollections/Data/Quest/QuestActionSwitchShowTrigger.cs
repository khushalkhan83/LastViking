using System.Collections;
using System.Collections.Generic;
using ActionsCollections;
using UnityEngine;

namespace DebugActions
{
    public class QuestActionSwitchShowTrigger : ActionBase
    {
        private readonly string _operationName;
        private EditorGameSettings EditorGameSettings => EditorGameSettings.Instance;

        public QuestActionSwitchShowTrigger(string name)
        {
            _operationName = name;
        }
        public override string OperationName => _operationName;

        public override void DoAction()
        {
            EditorGameSettings.ShowQuestTriggers = !EditorGameSettings.ShowQuestTriggers;
        }
    }
}
