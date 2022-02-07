using System.Collections.Generic;
using Core.Providers;
using NaughtyAttributes;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Game.Objectives.Stacks
{
    [CreateAssetMenu(fileName = "SO_Provider_objectivesStack_default", menuName = "Providers/ObjectivesStack", order = 1)]
    public class StacksProvider : ProviderScriptable<int, ObjectivesStack>
    {

        [SerializeField] private IntObjectiveStackDictionary _map = new IntObjectiveStackDictionary();

        [Space]
        [Header("Settings")]
        [SerializeField] private List<ObjectivesStack> _objectivesStacks;

        public int ElementsCount => _map.Count;

        public override ObjectivesStack this[int key] => _map[key];

        public int GetID(ObjectivesStack stack) //TODO: add try get value
        {
            var match = _map.FirstOrDefault(x => x.Value == stack).Key;

            return match;
        }


#if UNITY_EDITOR
        [Button]
        void GenerateMap()
        {
            bool cancel = !EditorUtility.DisplayDialog("Внимание", "Map будет сгенерирован заново. Это может нарушить обратную совместимость. Вы уверены?", "Да", "Нет");
            if (cancel) return;

            _map.Clear();

            for (int i = 0; i < _objectivesStacks.Count; i++)
            {
                _map.Add(i, _objectivesStacks[i]);
            }
        }
#endif
    }
}