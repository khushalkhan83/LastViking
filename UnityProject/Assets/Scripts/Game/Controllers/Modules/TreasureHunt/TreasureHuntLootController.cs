using Core;
using Core.Controllers;
using Core.Views;
using Game.Models;
using Game.Views;

namespace Game.Controllers
{
    public class TreasureHuntLootController : ITreasureHuntLootController, IController
    {
        [Inject] public TreasureHuntModel huntModel { get; private set; }
        [Inject] public EventLootModel eventLootModel { get; private set; }
        [Inject] public ViewsSystem ViewsSystem { get; private set; }
        [Inject] public PlayerDeathModel PlayerDeathModel { get; private set; }

        void IController.Enable()
        {
            huntModel.OnWaitActivate += HuntModel_OnWaitActivate;
            huntModel.OnBeginDigMode += StartDigModeHandler;
            huntModel.OnBeginRecive += HuntModel_OnBeginRecive;
            eventLootModel.onCustomLootRecived += ReciveLootHandler;
            PlayerDeathModel.OnRevival += OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim += OnRevivalPrelimHandler;
        }

        void IController.Start()
        {
            if (huntModel.state == TreasureHuntState.reciveBottle)
            {
                HuntModel_OnBeginRecive();
            }
            else if (huntModel.state == TreasureHuntState.waitActivate)
            {
                OpenView();
            }
        }

        void IController.Disable()
        {
            huntModel.OnWaitActivate -= HuntModel_OnWaitActivate;
            huntModel.OnBeginDigMode -= StartDigModeHandler;
            huntModel.OnBeginRecive -= HuntModel_OnBeginRecive;
            eventLootModel.onCustomLootRecived -= ReciveLootHandler;
            PlayerDeathModel.OnRevival -= OnRevivalHandler;
            PlayerDeathModel.OnRevivalPrelim -= OnRevivalPrelimHandler;

            if (huntModel.state == TreasureHuntState.reciveBottle)
            {
                eventLootModel.UnregisterCustomLoot(bottleKey);
            }
        }

        private void HuntModel_OnWaitActivate()
        {
            eventLootModel.UnregisterCustomLoot(TreasureHuntModel.kLootBottle);
            OpenView();
        }

        private void HuntModel_OnBeginRecive()
        {
            eventLootModel.RegisterCustomLoot(bottleKey, 100);
        }

        private void OnRevivalPrelimHandler() => UpdateViewsVisible();
        private void OnRevivalHandler() => UpdateViewsVisible();

        private void UpdateViewsVisible()
        {
            if (huntModel.state == TreasureHuntState.waitActivate)
            {
                OpenView();
            }
        }

        string bottleKey => TreasureHuntModel.kLootBottle;

        void ReciveLootHandler(string lootKey)
        {
            if (huntModel.state == TreasureHuntState.reciveBottle)
            {
                if (lootKey.Equals(bottleKey))
                {
                    huntModel.StartWaitActivateMode();
                }
            }
        }

        void StartDigModeHandler() => CloseView();

        public IView View { get; private set; }
        private void OpenView()
        {
            if (View == null)
            {
                View = ViewsSystem.Show<AimButtonView>(ViewConfigID.TreasureHuntInit);
                View.OnHide += OnHideHandler;
            }
        }

        private void OnHideHandler(IView view)
        {
            view.OnHide -= OnHideHandler;
            View = null;
        }

        private void CloseView()
        {
            if (View != null)
            {
                ViewsSystem.Hide(View);
            }
        }
    }
}
