using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class RepairController : IRepairController, IController
    {
        [Inject] public RepairingItemsModel RepairingItemsModel { get; private set; }
        [Inject] public GameUpdateModel GameUpdateModel { get; private set; }

        void IController.Enable()
        {
            RepairingItemsModel.OnRepairedItem += OnRepairedItemHandler;
            GameUpdateModel.OnUpdate += OnUpdate;
        }

        void IController.Start()
        {

        }

        void IController.Disable()
        {
            GameUpdateModel.OnUpdate -= OnUpdate;
            RepairingItemsModel.OnRepairedItem -= OnRepairedItemHandler;
        }

        private void OnUpdate()
        {
            RepairingItemsModel.RepairProcess(Time.deltaTime);
        }

        private void OnRepairedItemHandler(SavableItem item)
        {
            var durability = item.GetProperty("Durability");
            var repairCount = item.GetProperty("Repair count");
            var repairIndex = repairCount.Int.Default - repairCount.Int.Current;

            durability.Float.Current = (int)durability.Float.Default * RepairingItemsModel.PartRestoredDuratility(repairIndex);
            item.SetProperty(durability);
            repairCount.Int.Current--;
        }
    }
}
