using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Game.Objectives;
using System.Linq;
using Extensions;

namespace Game.Objectives.Stacks.Predefined.Tiers
{
    [CreateAssetMenu(fileName = "Tier", menuName = "UnityProject/Tier", order = 0)]
    public class Tier : ScriptableObject
    {
        [SerializeField] private bool compleated;

        [ReorderableList]
        [SerializeField] private List<ExtenedObjectivesStack> extendedStacks = new List<ExtenedObjectivesStack>();
        public List<ObjectivesStack> Stacks => extendedStacks.Select(x => x.stack).ToList();
        public bool Completed => compleated;
        public void Comleate() => compleated = true;
        // public void Reset() => Rest(false); //TODO: remove ?

        public void Reset(bool state)
        {
            compleated = state;
            Stacks.ForEach(x => x.RegenerateStack());
        }

        public bool CanReroll(int objectiveIndex)
        {
            bool outOfRange = extendedStacks.IndexOutOfRange(objectiveIndex);
            if (outOfRange)
            {
                Debug.LogError("Index out of range here");
                return true;
            }

            bool canReroll = extendedStacks[objectiveIndex].canReroll;
            return canReroll;
        }


        [System.Serializable]
        public class ExtenedObjectivesStack
        {
            public ObjectivesStack stack;
            public bool canReroll;
        }
    }
}