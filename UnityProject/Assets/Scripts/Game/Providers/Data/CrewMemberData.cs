using Game.Controllers;
using Game.QuestSystem.Data;
using RoboRyanTron.SearchableEnum;
using System;
using UnityEngine;

namespace Game.Providers
{
    [Serializable]
    public class CrewMemberData
    {
        #region Data
        #pragma warning disable 0649
        [SerializeField] private QuestData relatedQuest;
        
        #pragma warning restore 0649
        #endregion

        public QuestData RelatedQuest => relatedQuest;
    }
}
