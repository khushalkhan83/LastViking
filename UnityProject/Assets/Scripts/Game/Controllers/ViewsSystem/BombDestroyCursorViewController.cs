using Core;
using Core.Controllers;
using Game.Interactables;
using Game.Models;
using Game.Views;
using UltimateSurvival;
using static Game.Models.QuestsLifecycleModel;

namespace Game.Controllers
{
    public class BombDestroyCursorViewController : ViewControllerBase<BombDestroyCursorView>
    {
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }
        [Inject] public PlayerEventHandler PlayerEventHandler { get; private set; }
        [Inject] public ItemsDB ItemsDB { get; private set; }
        [Inject] public QuestsLifecycleModel QuestsLifecycleModel { get; private set; }
        [Inject] public InventoryOperationsModel InventoryOperationsModel { get; private set; }

        public BombDestroyEnter BombDestroyEnter { get; private set; }

        private bool BombCanBePlanted => QuestsLifecycleModel.EventOccured(QuestEvent.WaterfallAvaliable);

        private bool CantFindBombItemInDataBase => _bombItemId == -1;
        private const string _bombItemName = "weapon_bomb_small";
        private int _bombItemId = -1;

        protected override void Show()
        {
            View.OnInteract += OnDownHandler;
            GameUpdateModel.OnUpdate += OnUpdate;

            var item = ItemsDB.GetItem(_bombItemName);
            if (item != null)
                _bombItemId = item.Id;

            // unsubscribe on end time
            BombDestroyEnter = PlayerEventHandler.RaycastData.Value.GameObject.GetComponentInParent<BombDestroyEnter>();
        }

        protected override void Hide()
        {
            View.OnInteract -= OnDownHandler;
            GameUpdateModel.OnUpdate -= OnUpdate;
            if (BombDestroyEnter != null)
            {
                BombDestroyEnter.HideGhostBomb();
            }
        }

        private void OnUpdate()
        {
            if (BombDestroyEnter == null)
                return;

            if (BombDestroyEnter.BombPlanted && BombCanBePlanted)
            {
                View.SetBombPlantedTimerVisible(true);
                View.SetNobombVisible(false);
                View.SetUseBombVisible(false);
                var remainingTime = BombDestroyEnter.GetRemainingBombTime();
                View.SetBombFillAmount(remainingTime / BombDestroyEnter.DetonateTimeout);
                View.SetBombTimerText(TimeToString(remainingTime));
                BombDestroyEnter.HideGhostBomb();
            }
            else if (BombCanBePlanted && HasBomb())
            {
                View.SetBombPlantedTimerVisible(false);
                View.SetNobombVisible(false);
                View.SetUseBombVisible(true);
                BombDestroyEnter.ShowGhostBomb();
            }
            else
            {
                View.SetBombPlantedTimerVisible(false);
                View.SetNobombVisible(BombCanBePlanted);
                View.SetUseBombVisible(false);
            }
        }

        private string TimeToString(float time) => System.TimeSpan.FromSeconds(time).ToString(@"mm\:ss");

        private void OnDownHandler()
        {
            if (   BombCanBePlanted
                && BombDestroyEnter != null
                && BombDestroyEnter.BombPlanted == false
                && HasBomb())
            {
                RemoveBomb();
                BombDestroyEnter.PlantBomb();
            }
        }

        private bool HasBomb()
        {
            if(CantFindBombItemInDataBase) return false;

            return InventoryOperationsModel.PlayerHasItem(_bombItemId);
        }

        private void RemoveBomb()
        {
            InventoryOperationsModel.TryRemoveItem(_bombItemName);
        }
    }
}

