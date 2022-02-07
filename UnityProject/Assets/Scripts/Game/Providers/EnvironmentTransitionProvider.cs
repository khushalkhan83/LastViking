using Core.Providers;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Game.Providers
{
    [CreateAssetMenu(fileName = "SO_Providers_environmentTransitionsProvider_default", menuName = "Providers/Transitions", order = 0)]

    public class EnvironmentTransitionProvider : ProviderScriptable<int, EnvironmentTransition>
    {
        [SerializeField] private IntEnvironmentTransitionDictionary _map = new IntEnvironmentTransitionDictionary();

        public override EnvironmentTransition this[int key] => _map[key];

        public bool TryGetTransitionIndex(EnvironmentTransition transition, out int index)
        {
            index = -1;
            bool questExist = _map.ContainsValue(transition);
            if(!questExist) return false;

            var keyValuePair = _map.Where(x => x.Value == transition).FirstOrDefault();
            index = keyValuePair.Key;

            return true;
        }

        public IEnumerable<int> TransitionIndexes => _map.Keys;
    }
}
