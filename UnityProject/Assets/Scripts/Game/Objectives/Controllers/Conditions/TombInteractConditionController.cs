using Core;
using Game.Models;
using Game.Objectives.Data.Conditions.Static;
using Game.Views;

namespace Game.Objectives.Conditions.Controllers
{
    public class TombInteractConditionController : BaseConditionController<TombInteractConditionData, DoneConditionDataModel>
    {
        [Inject] public ViewsSystem ViewsSystem { get; private set; }

        protected override void Subscribe()
        {
            ViewsSystem.OnBeginShow.AddListener(ViewConfigID.TombPopup, OnShowTombPopup);
        }

        protected override void Unsubscribe()
        {
            ViewsSystem.OnBeginShow.RemoveListener(ViewConfigID.TombPopup, OnShowTombPopup);
        }

        private void OnShowTombPopup() => EventProcessing
            (
                data => true
                , model => model.Progress(true)
                , (data, model) => true
            );
    }
}
