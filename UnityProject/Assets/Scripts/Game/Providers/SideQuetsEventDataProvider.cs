using Core.Providers;
using UnityEngine;
using static Game.Models.QuestsLifecycleModel;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_quests_sideEventData_default", menuName = "Providers/Quests/SideEventData", order = 0)]
    public class SideQuetsEventDataProvider : ProviderScriptable<SideQuestEvent, SideQuestEventData>
    {
        [SerializeField] private SideQuestEventQuestEventDataDictionary _map = new SideQuestEventQuestEventDataDictionary();

        public override SideQuestEventData this[SideQuestEvent key] => _map[key];
    }
}
