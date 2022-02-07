using Core;
using Core.Controllers;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class ActivateWarmBarController : ViewEnableController<WarmBarView>, IController
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }

        public override ViewConfigID ViewConfigID => ViewConfigID.WarmBarViewConfig;
        public override bool IsCanShow => !PlayerHealthModel.IsDead;

        public override void Enable()
        {
            PlayerDeathModel.OnRevival += UpdateViewVisible;
            PlayerDeathModel.OnRevivalPrelim += UpdateViewVisible;
            
            UpdateViewVisible();
        }

        public override void Start()
        {
            
        }

        public override void Disable()
        {
            PlayerDeathModel.OnRevival -= UpdateViewVisible;
            PlayerDeathModel.OnRevivalPrelim -= UpdateViewVisible;
        }

    }
}
