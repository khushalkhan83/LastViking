
using Game.Models;

namespace Game.QuestSystem.Map.Triggers.TimelineSkipCheck
{
    public class ShelterUpgradeTimelineSkipCheck : TimelineSkipCheckBase
    {
        private ShelterUpgradeModel ShelterUpgradeModel => ModelsSystem.Instance._shelterUpgradeModel;

        public override bool Skip()
        {
            bool skip = ShelterUpgradeModel.UpgradedInThisChapter;
            return skip;
        }
    }
}