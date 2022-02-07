using Core;
using Game.AI;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;

namespace Game.Objectives.Conditions.Controllers
{
    public class BreakWorldObjectConditionController : BaseConditionController<BreakWorldObjectConditionData, CountConditionDataModel>
    {
        [Inject] public StatisticWorldObjectsNodel StatisticWorldObjectsNodel { get; private set; }

        protected override void Subscribe()
        {
            StatisticWorldObjectsNodel.OnKill += OnKillWorldObjectHandler;
        }

        protected override void Unsubscribe()
        {
            StatisticWorldObjectsNodel.OnKill -= OnKillWorldObjectHandler;
        }

        private void OnKillWorldObjectHandler(TargetID targetID, WorldObjectID worldObjectID) => EventProcessing
            (
                data => data.TargetID == targetID && data.WorldObjectID == worldObjectID
                , model => model.Progress(1)
                , (data, model) => data.Value <= model.Value
            );
    }
}
