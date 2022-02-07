using Core.Providers;
using Game.QuestSystem.Data.QuestTriggers;
using Game.QuestSystem.Map.Triggers;
using UnityEngine;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_questTriggers_default", menuName = "Providers/QuestTriggers", order = 0)]

    public class QuestTriggerProvider : ProviderScriptable<QuestTriggerType, GameObject> {
        [EnumNamedArray(typeof(QuestTriggerType),handleFirstAsNone:false)]
        [SerializeField] private GameObject[] _data;

        public override GameObject this[QuestTriggerType key] => _data[((int)(object)key)];
     }
}
