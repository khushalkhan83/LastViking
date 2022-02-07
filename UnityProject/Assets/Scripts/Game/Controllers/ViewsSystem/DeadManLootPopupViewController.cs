using Core;
using Game.Models;
using Game.Progressables;
using UnityEngine;
using System.Linq;

namespace Game.Controllers
{
    public class DeadManLootPopupViewController : TreasureLootPopupViewController
    {
        [Inject] public DeadManLootChestModel DeadManLootChestModel { get; set; }

        protected override void InitializeLootItmes()
        {
            if (DeadManLootChestModel.NeedResetItems)
            {
                ClearLootItems();
                GenerateItems();
                DeadManLootChestModel.NeedResetItems = false;
            }
        }

        protected override void OnChestOpen()
        {
            DeadManLootChestModel.OnChestOpen();
        }

        protected override void OnItemsTaken() 
        {
            base.OnItemsTaken();
            DeadManLootChestModel.Unlock = false;
        }

        protected override bool UseBigChestSize => true;

        public override Vector3 DropChestPosition()
        {
            var spawners = FindObjectsOfType<NonSavableProgressSpawner>().ToList();
            var deadManChestSpawner = spawners.Find(x => x.WorldObjectID == WorldObjectID.loot_container_epic);

            if(deadManChestSpawner == null) 
                return base.DropChestPosition();

            return deadManChestSpawner.transform.position + Vector3.down;
        }
    }
}
