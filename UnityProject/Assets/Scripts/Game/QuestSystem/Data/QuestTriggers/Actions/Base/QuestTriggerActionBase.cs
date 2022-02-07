using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.QuestSystem.Data.QuestTriggers.Actions
{
    [Serializable]
    public abstract class QuestTriggerActionBase
    {
        public abstract void Do();
    }
}