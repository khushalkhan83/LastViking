using Core;
using Extensions;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class CraftButtonController : ViewEnableController<CraftInteractView>
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }

        [Inject] public CraftButtonViewModel CraftButtonViewModel { get; private set; }

        public override bool IsCanShow => false;
        public override ViewConfigID ViewConfigID => ViewConfigID.CraftInteract;

        public override void Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;

            CraftButtonViewModel.OnGetButton += OnGetButtonHandler;

            UpdateViewVisible();
        }

        public override void Start() { }

        public override void Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;

            CraftButtonViewModel.OnGetButton -= OnGetButtonHandler;

            Hide();
        }

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();
        private GameObject OnGetButtonHandler() => View.CheckNull()?.Button;
    }
}
