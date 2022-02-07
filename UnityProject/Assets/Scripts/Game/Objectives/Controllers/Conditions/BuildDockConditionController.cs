using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using Game.Views;

namespace Game.Objectives.Conditions.Controllers
{
    public class BuildDockConditionController : BaseConditionController<BuildDockConditionData, DoneConditionDataModel>
    {
        [Inject] public ConstructionDockModel ConstructionDockModel { get; private set; }

        protected override void Subscribe()
        {
            ConstructionDockModel.OnDockBuilded += OnDockBuilded;
        }

        protected override void Unsubscribe()
        {
            ConstructionDockModel.OnDockBuilded -= OnDockBuilded;
        }

        private void OnDockBuilded() => EventProcessing
            (
                data => true
                , model => model.Progress(true)
                , (data, model) => true
            );
    }
}
