using System.Linq;
using Core;
using Core.Controllers;
using Game.Models;
using UltimateSurvival;
using UnityEngine;

namespace Game.Controllers
{
    public class TombsController : ITombsController, IController
    {
        [Inject] public TombsModel TombsModel {get; private set;}
        [Inject] public WorldObjectCreator WorldObjectCreator {get; private set;}
        [Inject] public PlayerEventHandler PlayerEventHandler {get; private set;}
        [Inject] public GameTimeModel GameTimeModel {get; private set;}
        [Inject] public InventoryModel InventoryModel {get; private set;}
        [Inject] public HotBarModel HotBarModel {get; private set;}
        [Inject] public StatisticsModel StatisticsModel {get; private set;}
        [Inject] public PlayerProfileModel PlayerProfileModel {get; private set;}
        [Inject] public SheltersModel SheltersModel {get; private set;}

        [Inject(true)] public PlayerRespawnPoints PlayerRespawnPoints {get; private set;}

        void IController.Enable() 
        {
            TombsModel.OnCreateTomb += OnCreateTombHanler;
        }

        void IController.Start() 
        {
        }

        void IController.Disable() 
        {
            TombsModel.OnCreateTomb -= OnCreateTombHanler;
        }


        private void OnCreateTombHanler()
        {
            var tombTargetTransform = GetTombSpawnTransform();

            var worldObject = WorldObjectCreator.Create(WorldObjectID.Tomb, tombTargetTransform.position, tombTargetTransform.rotation);
            var tombModel = worldObject.GetComponent<TombModel>();
            var inventoryItems = InventoryModel.ItemsContainer.Cells.Where(x => x.IsHasItem).Select(x => x.Item);
            var hotBarItems = HotBarModel.ItemsContainer.Cells.Where(x => x.IsHasItem && x.Item.Id != HotBarModel.DefaltItemID).Select(x => x.Item);

            var shelterLevel = 0;
            if (SheltersModel.ShelterActive != ShelterModelID.None)
            {
                shelterLevel = SheltersModel.ShelterModel.Level;
            }

            tombModel.Initialize(inventoryItems.Union(hotBarItems), GameTimeModel.GameDurationTicks - StatisticsModel.StartAliveTimeTicks, StatisticsModel.KilledAll, "Player", PlayerProfileModel.Index, SheltersModel.ShelterActive, shelterLevel);
        }

        private Transform GetTombSpawnTransform()
        {
            Transform tombTargetTransform;

            switch (TombsModel.Creation)
            {
                default:
                case TombsModel.CreationType.Default:
                    tombTargetTransform = PlayerEventHandler.transform;
                    break;
                case TombsModel.CreationType.RespawnPoint:
                    if(PlayerRespawnPoints == null || PlayerRespawnPoints.InitPlayerPoint == null)   
                        tombTargetTransform = PlayerEventHandler.transform;
                    else
                        tombTargetTransform = PlayerRespawnPoints.InitPlayerPoint;
                    break;
            }
            return tombTargetTransform;
        }
    }
}
