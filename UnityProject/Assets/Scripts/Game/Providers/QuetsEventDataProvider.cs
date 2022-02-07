using Core.Providers;
using Game.QuestSystem.Data;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using static Game.Models.QuestsLifecycleModel;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_quests_eventData_default", menuName = "Providers/Providers/Quests/EventData", order = 0)]
    public class QuetsEventDataProvider : ProviderScriptable<QuestEvent, QuestEventData>
    {
        [SerializeField] private QuestEventQuestEventDataDictionary _map = new QuestEventQuestEventDataDictionary();

        public override QuestEventData this[QuestEvent key] => _map[key];
    }
}