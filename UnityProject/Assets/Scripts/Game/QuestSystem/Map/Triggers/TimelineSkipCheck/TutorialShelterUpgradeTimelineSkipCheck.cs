
using Game.Models;

namespace Game.QuestSystem.Map.Triggers.TimelineSkipCheck
{
    public class TutorialShelterUpgradeTimelineSkipCheck : TimelineSkipCheckBase
    {
        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;

        public override bool Skip()
        {
            bool skip = ShelterUpgradeModel.IsConstructed;
            return skip;
        }
    }
}