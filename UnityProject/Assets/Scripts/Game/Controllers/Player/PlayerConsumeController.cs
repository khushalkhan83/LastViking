using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class PlayerConsumeController : IPlayerConsumeController, IController
    {
        [Inject] public PlayerConsumeModel PlayerConsumeModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        void IController.Enable()
        {
            PlayerConsumeModel.OnStartConsumeProcess += OnStartConsumeProcessHanle;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            PlayerConsumeModel.OnStartConsumeProcess -= OnStartConsumeProcessHanle;
            PlayerConsumeModel.OnEndConsumeProcess -= OnEndConsumeProcessHanle;
            GameUpdateModel.OnUpdate -= ConsumeProcess;
        }

        private void OnStartConsumeProcessHanle()
        {
            PlayerConsumeModel.OnStartConsumeProcess -= OnStartConsumeProcessHanle;
            PlayerConsumeModel.OnEndConsumeProcess += OnEndConsumeProcessHanle;
            GameUpdateModel.OnUpdate += ConsumeProcess;
        }

        private void OnEndConsumeProcessHanle()
        {
            PlayerConsumeModel.OnEndConsumeProcess -= OnEndConsumeProcessHanle;
            GameUpdateModel.OnUpdate -= ConsumeProcess;
            PlayerConsumeModel.OnStartConsumeProcess += OnStartConsumeProcessHanle;
        }

        private void ConsumeProcess() => PlayerConsumeModel.ConsumeProcess();
    }
}
