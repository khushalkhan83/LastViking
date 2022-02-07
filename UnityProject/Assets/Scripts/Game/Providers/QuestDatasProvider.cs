using Core.Providers;
using Game.QuestSystem.Data;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Game.Providers
{
    // [CreateAssetMenu(fileName = "SO_Providers_questDatasProvider_default", menuName = "Providers/QuestDatas", order = 0)]

    public class QuestDatasProvider : ProviderScriptable<int, QuestData>
    {
        [SerializeField] private IntQuestDataDictionary _map = new IntQuestDataDictionary();

        public override QuestData this[int key] => _map[key];

        public bool TryGetQuestIndex(QuestData quest, out int index)
        {
            index = -1;
            bool questExist = _map.ContainsValue(quest);
            if(!questExist) return false;

            var keyValuePair = _map.Where(x => x.Value == quest).FirstOrDefault();
            index = keyValuePair.Key;

            return true;
        }

        public IEnumerable<int> QuestsIndexes => _map.Keys;
    }
}
