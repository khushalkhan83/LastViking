using System;
using Game.QuestSystem.Data.QuestTriggers.Actions;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.QuestSystem.Data.QuestTriggers
{
    [Serializable]
    public class QuestTriggerWrapper
    {

        [TableList]
        public QuestTriggerData data;

        public bool Validate()
        {
            // return data.Validate();
            return true;
        }
    }
}