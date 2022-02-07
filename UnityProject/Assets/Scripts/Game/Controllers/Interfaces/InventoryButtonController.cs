using Core;
using Extensions;
using Game.Models;
using Game.Views;
using UnityEngine;

namespace Game.Controllers
{
    public class InventoryButtonController : ViewEnableController<InventoryInteractView>
    {
        [Inject] public PlayerHealthModel PlayerHealthModel { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }
        [Inject] public InventoryButtonViewModel InventoryButtonViewModel { get; private set; }
        public override ViewConfigID ViewConfigID => ViewConfigID.InventoryInteract;

        public override bool IsCanShow => !PlayerHealthModel.IsDead;

        public override void Start()
        {
            
        }
        public override void Enable()
        {
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
            InventoryButtonViewModel.OnGetButton += GetButtonHandler;

            UpdateViewVisible();
        }
        public override void Disable()
        {
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;
            InventoryButtonViewModel.OnGetButton -= GetButtonHandler;

            Hide();
        }

        private void OnRevivalPrelimHandler() => UpdateViewVisible();
        private void OnRevivalHandler() => UpdateViewVisible();
        private GameObject GetButtonHandler() => View.CheckNull()?.Button;
    }
}