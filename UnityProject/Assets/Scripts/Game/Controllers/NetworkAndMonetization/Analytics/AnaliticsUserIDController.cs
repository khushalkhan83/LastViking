using Core;
using Game.Models;
using Game.Views;
using UnityEngine.Analytics;

namespace Game.Controllers
{
    public class AnaliticsUserIDController : ViewEnableController<QuestionPopupView>, IAnaliticsUserIDController
    {
        [Inject] public AnaliticsUserIDModel AnaliticsUserIDModel { get; private set; }
        [Inject] public StorageModel StorageModel { get; private set; }

        public override ViewConfigID ViewConfigID => ViewConfigID.UserIDQuestionPopupConfig;

        public override bool IsCanShow => true;

        public override void Enable()
        {
            AnaliticsUserIDModel.OnShowPopup += OnShowPopupHandler;
            StorageModel.TryProcessing(AnaliticsUserIDModel._Data);
            AnaliticsUserIDModel.SetID(AnalyticsSessionInfo.userId);
        }

        public override void Start() { }

        public override void Disable()
        {
            AnaliticsUserIDModel.OnShowPopup -= OnShowPopupHandler;
        }

        private void OnShowPopupHandler()
        {
            Show();
        }
    }
}
