using Core;
using Core.Controllers;
using Game.Models;

namespace Game.Controllers
{
    public class CheatController : ICheatController, IController
    {
        [Inject] public CheatModel CheatModel { get; private set; }
        [Inject] public CoinsModel CoinsModel { get; private set; }

        void IController.Start() { }

        void IController.Enable() 
        {
            CheatModel.Init();

            PlayerIsCheaterCheck();
            CoinsModel.OnChange += PlayerIsCheaterCheck;
        }
        void IController.Disable() 
        {
            CoinsModel.OnChange -= PlayerIsCheaterCheck;
        }

        private void PlayerIsCheaterCheck()
        {
            if(CheatModel.PlayerIsCheater) return;

            if(CoinsModel.Coins >= CheatModel.CoinsLimit)
            {
                CheatModel.MarkPlayerAsCheater();
            }
        }
    }
}