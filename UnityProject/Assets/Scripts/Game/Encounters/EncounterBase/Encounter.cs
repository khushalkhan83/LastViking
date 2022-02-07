using Game.Models;
using UnityEngine;

namespace Encounters
{
    public class Encounter : EncounterBase
    {
        private readonly EncounterConfig config;
        private readonly IEncounterStorage storage;

        public Encounter(EncounterConfig config, IEncounterStorage model)
        {
            this.config = config;
            this.storage = model;
        }

        public override bool CanOccure => !(cooldown > 0) && TutorialModel.IsComplete && IsUnlocked;

        public override float chanceWeight => config.chanceWeight;

        protected override IEncounterStorage model => storage;

        protected override float resetTimeSeconds => config.resetTime;

        protected override int neededStoryChapter => config.requiaredStoryChapter;
    }
}