using System.Collections.Generic;
using System.Linq;
using Game.Models;
using Game.Objectives.Stacks.Predefined.Tiers;
using NaughtyAttributes;
using UnityEngine;

namespace Game.Objectives.Stacks.Predefined
{
    [System.Serializable]
    public class PredefinedStacks
    {
        private ObjectivesProgressModel ObjectivesProgressModel => ModelsSystem.Instance._objectivesProgressModel;

        [ReorderableList]
        [SerializeField] private List<Tier> predefinedTiers = new List<Tier>();
        public Tier SelectedTier { get; private set; }
        public bool Compleated { get; private set; }

        public bool FirstInit
        {
            get => !ObjectivesProgressModel.InitedRequiredTiers();
            set => ObjectivesProgressModel.SetInitedRequiredTiers(!value);
        }
        public List<bool> RequiredTiersStates { get => predefinedTiers.Select(x => x.Completed).ToList(); }

        public int TiersCount() => predefinedTiers.Count;

        public void Reset()
        {
            // loading from model
            for (int i = 0; i < predefinedTiers.Count; i++)
            {
                bool state = ObjectivesProgressModel.GetTiersState(i);
                predefinedTiers[i].Reset(state);
            }

            SetNextSelectedTier();
        }

        public void CompleateSelectedTierAndSetNext()
        {
            if (SelectedTier != null)
                SelectedTier.Comleate();
            SetNextSelectedTier();
        }

        public void SetNextSelectedTier()
        {
            var nextTier = predefinedTiers.FirstOrDefault(x => x.Completed == false);
            SelectedTier = nextTier;
            Compleated = nextTier == null;
        }

        public bool TrtGetCanRerollObjective(int objectiveIndex, out bool canReroll)
        {
            canReroll = true;
            if (SelectedTier == null) return false;

            canReroll = SelectedTier.CanReroll(objectiveIndex);
            return true;
        }

    }
}